﻿namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Converts San move into Move object<br/>
    /// Long algebraic notation is also acceptable
    /// </summary>
    /// <param name="move">San move that will be converted</param>
    /// <returns>Move object according to given san</returns>
    /// <exception cref="ArgumentNullException">move was null</exception>
    /// <exception cref="ArgumentException">Given move didn't match the Regex pattern</exception>
    /// <exception cref="ChessSanNotFoundException">Given SAN move is not valid for current board positions</exception>
    /// <exception cref="ChessSanTooAmbiguousException">Given SAN move is too ambiguous between multiple moves</exception>
    public Move San(string move)
    {
        if (move is null)
            throw new ArgumentNullException(nameof(move));

        string pattern = @"(^([PNBRQK])?([a-h])?([1-8])?(x|X|-)?([a-h][1-8])(=[NBRQ]| ?e\.p\.)?|^O-O(-O)?)(\+|\#|\$)?$";

        var matches = Regex.Matches(move, pattern);

        if (matches.Count == 0)
            throw new ArgumentException("SAN Move should match pattern: " + pattern);

        Move moveOut = new();
        Position originalPos = new();
        bool isCapture = false;

        foreach (var group in matches[0].Groups.Values)
        {
            if (!group.Success) continue;

            switch (group.Name)
            {
                case "1":
                    if (group.Value == "O-O" || group.Value == "O-O-O")
                    {
                        moveOut.Parameter = MoveParameter.FromString(group.Value);
                        if (Turn == PieceColor.White)
                        {
                            originalPos = new("e1");
                            if (group.Value == "O-O")
                                moveOut.NewPosition = new("h1");
                            else if (group.Value == "O-O-O")
                                moveOut.NewPosition = new("a1");
                        }
                        else if (Turn == PieceColor.Black)
                        {
                            originalPos = new("e8");
                            if (group.Value == "O-O")
                                moveOut.NewPosition = new("h8");
                            else if (group.Value == "O-O-O")
                                moveOut.NewPosition = new("a8");
                        }
                        if (!IsValidMove(new Move(originalPos, moveOut.NewPosition)))
                            throw new ChessSanNotFoundException(this, move);
                    }
                    break;
                case "2":
                    moveOut.Piece = new Piece(Turn, PieceType.FromChar(group.Value[0]));
                    break;
                case "3":
                    originalPos.X = Position.FromFile(group.Value[0]);
                    break;
                case "4":
                    originalPos.Y = Position.FromRank(group.Value[0]);
                    break;
                case "5":
                    if (group.Value == "x" || group.Value == "X")
                        isCapture = true;
                    break;
                case "6":
                    moveOut.NewPosition = new Position(group.Value);
                    break;
                case "7":
                    moveOut.Parameter = MoveParameter.FromString(group.Value.Trim());
                    break;
                case "9":
                    switch (group.Value)
                    {
                        case "+":
                            moveOut.IsCheck = true;
                            break;
                        case "#":
                            moveOut.IsCheck = true; moveOut.IsMate = true;
                            break;
                        case "$":
                            moveOut.IsMate = true;
                            break;
                    }
                    break;
            }
        }

        moveOut.Piece ??= new Piece(Turn, PieceType.Pawn);

        if (isCapture && this[moveOut.NewPosition] is not null)
            moveOut.CapturedPiece = this[moveOut.NewPosition];

        if (!originalPos.HasValue)
        {
            var amb = GetMovesOfPieceOnPosition(moveOut.Piece, moveOut.NewPosition, this).ToList();

            if (originalPos.HasValueX)
            {
                amb = amb.Where(m => m.OriginalPosition.X == originalPos.X).ToList();

                if (amb.Count == 1)
                    originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;

                else ThrowException(amb.Count, amb);
            }
            else if (originalPos.HasValueY)
            {
                amb = amb.Where(m => m.OriginalPosition.Y == originalPos.Y).ToList();

                if (amb.Count == 1)
                    originalPos.X = amb.ElementAt(0).OriginalPosition.X;

                else ThrowException(amb.Count, amb);
            }
            else
            {
                if (amb.Count == 1)
                {
                    originalPos.X = amb.ElementAt(0).OriginalPosition.X;
                    originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;
                }
                else ThrowException(amb.Count, amb);
            }
            void ThrowException(int count, List<Move> moves)
            {
                if (count < 1)
                    throw new ChessSanNotFoundException(this, move);
                else if (count > 1)
                    throw new ChessSanTooAmbiguousException(this, move, moves.ToArray());
            }
        }

        moveOut.OriginalPosition = originalPos;
        San(moveOut);

        return moveOut;
    }

    /// <summary>
    /// Converts move object to string-move in Standard Algebraic notation
    /// </summary>
    /// <param name="move">Move to be converted</param>
    /// <returns>Move in SAN</returns>
    /// <exception cref="ArgumentNullException">Given move was null or didn't have valid value</exception>
    public string San(Move move)
    {
        if (move is null || !move.HasValue)
            throw new ArgumentNullException(nameof(move));

        string sMove = "";

        if (move.Parameter == MoveParameter.CastleKing || move.Parameter == MoveParameter.CastleQueen)
        {
            sMove = move.Parameter.AsShortString;
            goto CheckOrMateValidation;
        }

        if (move.Piece.Type != PieceType.Pawn)
        {
            sMove += char.ToUpper(move.Piece.Type.AsChar);

            // Only rook, knight, bishop(second from promotion) and queen(second from promotion) can have ambiguous moves
            if (move.Piece.Type != PieceType.King)
                sMove += HandleAmbiguousMovesNotation(move, this);
        }

        if (move.CapturedPiece is not null)
        {
            if (move.Piece.Type == PieceType.Pawn)
                sMove += move.OriginalPosition.File();

            sMove += "x";
        }

        sMove += move.NewPosition;

        if (move.Parameter == MoveParameter.PawnPromotion
         || move.Parameter == MoveParameter.PromotionToQueen
         || move.Parameter == MoveParameter.PromotionToRook
         || move.Parameter == MoveParameter.PromotionToBishop
         || move.Parameter == MoveParameter.PromotionToKnight)
            sMove += move.Parameter.AsShortString;

        // Not required
        //else if (move.Parameter == MoveParameter.EnPassant)
        //    sMove += " " + move.Parameter.AsShortString;

        CheckOrMateValidation:
        if (move.IsCheck && move.IsMate) sMove += "#";
        else if (move.IsCheck) sMove += "+";
        else if (move.IsMate) sMove += "$";

        move.San = sMove;
        return sMove;
    }

    /// <summary>
    /// Load Chess game from Forsyth-Edwards Notation<br/>
    /// ex.:<br/>
    /// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    /// </summary>
    /// <param name="fen">FEN string</param>
    /// <exception cref="ArgumentException">Given FEN string didn't match the Regex pattern</exception>
    public void LoadFen(string fen)
    {
        Fen = new FenBoard(fen);
        pieces = Fen.Pieces;
        ExecutedMoves.Clear();
        moveIndex = -1;
        endGame = null;

        headers.Clear();
        headers.Add("Variant", "From Position");
        headers.Add("FEN", fen);

        HandleKingChecked();
        HandleEndGame();
    }

    /// <summary>
    /// Load Chess game from Portable Game Notation<br/>
    /// ex.:<br/>
    /// [Event "Live Chess"]<br/>
    /// [Site "Chess.com"]<br/>
    /// [White "Milan1905"]<br/>
    /// [Black "Geras1mleo"]<br/>
    /// [Result "1-0"]<br/>
    /// [WhiteElo "1006"]<br/>
    /// [BlackElo "626"]<br/>
    /// [EndTime "11:58:56 PST"]<br/>
    /// [Termination "Milan1905 won by resignation"]<br/>
    /// <br/>
    /// 1. e4 e5 2. Nf3 Nf6 3. Nc3 Nc6 4. Bb5 Bc5 5. Bxc6 bxc6 6. Nxe5 Bxf2+ 7. Kxf2 O-O
    /// 8. d4 d5 (8... c5 9. b4 (9. a3 c4 10. b4 cxb3) 9... c4) 9. exd5 cxd5 10. Nc6
    /// Ng4+ 11. Kg1 Qf6 12. Qf1 Qxc6 13. h3 Nf6 14. Bg5 Qb6 15. Bxf6 Qxf6 16. Qxf6 gxf6
    /// 17. Nxd5 Rb8 18. Nxf6+ Kh8 19. b3 Rb4 20. c3 Bb7 21. cxb4 1-0
    /// </summary>
    /// <param name="pgn">PGN string</param>
    public void LoadPgn(string pgn)
    {
        SetChessBeginSituation();
        Fen = null;
        ExecutedMoves.Clear();
        headers.Clear();
        moveIndex = -1;
        endGame = null;

        var headersMatches = Regex.Matches(pgn, @"\[(.*?) ("".*?"")\]");

        // Adding headers
        for (int i = 0; i < headersMatches.Count; i++)
        {
            string key = headersMatches[i].Groups[1].Value;
            string value = headersMatches[i].Groups[2].Value[1..^1];

            if (key.ToLower() == "fen")
                key = key.ToUpper();

            headers.Add(key, value);
        }

        // Loading fen if exist
        if (headers.TryGetValue("FEN", out var fen))
        {
            Fen = new FenBoard(fen);
            pieces = Fen.Pieces;
        }

        // Remove all alternatives now
        var alternatives = Regex.Matches(pgn, @"\(.*?\)");
        // Todo...
        // Alternative moves??? objects for each variant linked list?
        for (int i = 0; i < alternatives.Count; i++)
            pgn = pgn.Replace(alternatives[i].Value, "");

        var movesMatches = Regex.Matches(pgn, @"([PNBRQK]?[a-h]?[1-8]?[xX-]?[a-h][1-8](=[NBRQ]| ?e\.p\.)?|O-O(?:-O)?)[+#$]?");

        for (int i = 0; i < movesMatches.Count; i++)
        {
            var move = San(movesMatches[i].Value);
            if (IsValidMove(move, this, false, true))
            {
                San(move);
                ExecutedMoves.Add(move);
                DropPieceToNewPosition(move, true);
                moveIndex = ExecutedMoves.Count - 1;
            }
        }

        HandleKingChecked();
        HandleEndGame();

        if (!IsEndGame)
        {
            if (pgn.Contains("1-0"))
                Resign(PieceColor.Black);

            else if (pgn.Contains("0-1"))
                Resign(PieceColor.White);

            else if (pgn.Contains("1/2-1/2"))
                Draw();
        }
    }

    /// <summary>
    /// Generates Fen string of current board
    /// </summary>
    public string ToFen()
    {
        return new FenBoard(this).ToString();
    }

    /// <summary>
    /// Generates PGN string of current board
    /// </summary>
    public string ToPgn()
    {
        string pgn = "";

        foreach (var header in headers)
            pgn += '[' + header.Key + @" """ + header.Value + '"' + ']' + '\n';

        pgn += '\n';

        moveIndex = -1;

        for (int i = 0, count = 0; i < ExecutedMoves.Count; i++)
        {
            if (count != GetFullMovesCount(this))
            {
                count = GetFullMovesCount(this);
                
                if (i != 0) pgn += ' ';

                pgn += count + ".";
            }

            if (moveIndex == -1)
            {
                if (LoadedFromFEN && Fen.Turn == PieceColor.Black)
                    pgn += "..";
            }

            pgn += ' ' + ExecutedMoves[i].San;

            Next();
        }

        Last();

        return pgn;
    }

    /// <summary>
    /// Generates ASCII string of current board
    /// </summary>
    public string ToAscii(bool displayFull = false)
    {
        string ascii = "   ┌────────────────────────┐\n";

        for (int i = 8 - 1; i >= 0; i--)
        {
            ascii += "  " + (i + 1) + "│";
            for (int j = 0; j < 8; j++)
            {
                ascii += ' ';

                if (pieces[i, j] is not null)
                    ascii += pieces[i, j].ToFenChar();
                else
                    ascii += '.';

                ascii += ' ';
            }
            ascii += "│\n";
        }

        ascii += "   └────────────────────────┘\n";
        ascii += "     a  b  c  d  e  f  g  h  \n";

        if (displayFull)
        {
            ascii += '\n';

            ascii += "  Turn: " + Turn + '\n';

            if (WhiteCaptured.Length > 0)
                ascii += "  White Captured: " + string.Join(", ", WhiteCaptured.Select(p => p.ToFenChar())) + '\n';
            if (BlackCaptured.Length > 0)
                ascii += "  Black Captured: " + string.Join(", ", BlackCaptured.Select(p => p.ToFenChar())) + '\n';
        }

        return ascii;
    }

    private static string HandleAmbiguousMovesNotation(Move move, ChessBoard board)
    {
        var amb = GetMovesOfPieceOnPosition(move.Piece, move.NewPosition, board).Where(m => m.OriginalPosition != move.OriginalPosition).ToList();
        var origPos = move.OriginalPosition.ToString();

        if (amb.Count == 0)
            return "";
        else if (amb.Count == 1)
        {
            if (amb[0].OriginalPosition.X == move.OriginalPosition.X)
                return origPos[1].ToString();
            else
                return origPos[0].ToString();
        }
        else
        {
            var sOut = "";

            if (amb.Any(m => m.OriginalPosition.X == move.OriginalPosition.X))
                sOut += origPos[1];

            if (amb.Any(m => m.OriginalPosition.Y == move.OriginalPosition.Y))
                sOut += origPos[0];

            return sOut;
        }
    }

    private static Move[] GetMovesOfPieceOnPosition(Piece piece, Position newPosition, ChessBoard board)
    {
        var moves = new List<Move>();
        Move move;

        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (board.pieces[i, j] is not null && board.pieces[i, j].Color == piece.Color && board.pieces[i, j].Type == piece.Type)
                {
                    // if original pos == new pos
                    if (newPosition.Y == i && newPosition.X == j) continue;

                    move = new Move(new() { Y = i, X = j }, newPosition) { Piece = piece };

                    if (IsValidMove(move, board) && !IsKingCheckedValidation(move, piece.Color, board))
                        moves.Add(move);
                }
            }
        }

        return moves.ToArray();
    }

    private static int GetHalfMovesCount(ChessBoard board)
    {
        int index = board.ExecutedMoves.GetRange(0, board.moveIndex + 1).FindLastIndex(m => m.CapturedPiece is not null || m.Piece.Type == PieceType.Pawn);

        if (board.LoadedFromFEN && index < 0)
            return board.Fen.HalfMoves + board.moveIndex + 1;

        if (index >= 0)
            return board.moveIndex - index;
        else
            return board.moveIndex + 1;
    }

    private static int GetFullMovesCount(ChessBoard board)
    {
        var count = 0;

        if (board.LoadedFromFEN)
            count += (board.Fen.FullMoves * 2) + (board.Fen.Turn == PieceColor.Black ? 1 : 0) - 2;

        return (board.moveIndex + count + 3) / 2;
    }

    internal class FenBoard
    {
        private readonly Piece?[,] pieces;

        /// <summary>
        /// "Begin Situation"
        /// </summary>
        public Piece?[,] Pieces => (Piece?[,])pieces.Clone();
        public PieceColor Turn { get; }

        public bool CastleWK { get; }
        public bool CastleWQ { get; }
        public bool CastleBK { get; }
        public bool CastleBQ { get; }

        public Position EnPassant { get; }

        /// <summary>
        /// Count since the last pawn advance or piece capture
        /// </summary>
        public int HalfMoves { get; }
        /// <summary>
        /// Black moves Count
        /// </summary>
        public int FullMoves { get; }

        public Piece[] WhiteCaptured { get; }
        public Piece[] BlackCaptured { get; }

        public FenBoard(ChessBoard board)
        {
            pieces = board.pieces;
            Turn = board.Turn;
            CastleWK = HasRightToCastle(PieceColor.White, MoveParameter.CastleKing, board);
            CastleWQ = HasRightToCastle(PieceColor.White, MoveParameter.CastleQueen, board);
            CastleBK = HasRightToCastle(PieceColor.Black, MoveParameter.CastleKing, board);
            CastleBQ = HasRightToCastle(PieceColor.Black, MoveParameter.CastleQueen, board);
            EnPassant = LastMoveEnPassantPosition(board);
            HalfMoves = GetHalfMovesCount(board);
            FullMoves = GetFullMovesCount(board);
        }

        public FenBoard(string fen)
        {
            string pattern = @"^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+) ([b|w]) (-|[K|Q|k|q]{1,4}) (-|[a-h][36]) (\d+ \d+)$";

            var matches = Regex.Matches(fen, pattern);

            if (matches.Count == 0)
                throw new ArgumentException("FEN should match pattern: " + pattern);

            pieces = new Piece[8, 8];

            foreach (var group in matches[0].Groups.Values)
            {
                switch (group.Name)
                {
                    case "1":
                        int x = 0, y = 7;
                        for (int i = 0; i < group.Length; i++)
                        {
                            if (group.Value[i] == '/')
                            {
                                y--;
                                x = 0;
                                continue;
                            }
                            if (x < 8)
                                if (char.IsLetter(group.Value[i]))
                                {
                                    pieces[y, x] = new Piece(group.Value[i]);
                                    x++;
                                }
                                else if (char.IsDigit(group.Value[i]))
                                    x += int.Parse(group.Value[i].ToString());
                        }
                        break;
                    case "3":
                        Turn = PieceColor.FromChar(group.Value[0]);
                        break;
                    case "4":
                        if (group.Value != "-")
                        {
                            if (group.Value.Contains('K'))
                                CastleWK = true;
                            if (group.Value.Contains('Q'))
                                CastleWQ = true;
                            if (group.Value.Contains('k'))
                                CastleBK = true;
                            if (group.Value.Contains('q'))
                                CastleBQ = true;
                        }
                        break;
                    case "5":
                        if (group.Value == "-")
                            EnPassant = new();
                        else
                            EnPassant = new Position(group.Value);
                        break;
                    case "6":
                        (HalfMoves, FullMoves) = group.Value.Split(' ').Select(s => int.Parse(s)).ToArray();
                        break;
                }
            }

            var wcap = new List<Piece>();
            var bcap = new List<Piece>();

            var fpieces = pieces.Cast<Piece>().Where(p => p is not null);

            // Calculating missing pieces on according begin pieces in fen
            // Math.Clamp() for get max/min taken figures (2 queens possible)
            wcap.AddRange(Enumerable.Range(0, Math.Clamp(8 - fpieces.Where(p => p.Type == PieceType.Pawn && p.Color == PieceColor.White).Count(), 0, 8)).Select(p => new Piece(PieceColor.White, PieceType.Pawn)));
            wcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Rook && p.Color == PieceColor.White).Count(), 0, 2)).Select(p => new Piece(PieceColor.White, PieceType.Rook)));
            wcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Bishop && p.Color == PieceColor.White).Count(), 0, 2)).Select(p => new Piece(PieceColor.White, PieceType.Bishop)));
            wcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Knight && p.Color == PieceColor.White).Count(), 0, 2)).Select(p => new Piece(PieceColor.White, PieceType.Knight)));
            wcap.AddRange(Enumerable.Range(0, Math.Clamp(1 - fpieces.Where(p => p.Type == PieceType.Queen && p.Color == PieceColor.White).Count(), 0, 1)).Select(p => new Piece(PieceColor.White, PieceType.Queen)));

            bcap.AddRange(Enumerable.Range(0, Math.Clamp(8 - fpieces.Where(p => p.Type == PieceType.Pawn && p.Color == PieceColor.Black).Count(), 0, 8)).Select(p => new Piece(PieceColor.Black, PieceType.Pawn)));
            bcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Rook && p.Color == PieceColor.Black).Count(), 0, 2)).Select(p => new Piece(PieceColor.Black, PieceType.Rook)));
            bcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Bishop && p.Color == PieceColor.Black).Count(), 0, 2)).Select(p => new Piece(PieceColor.Black, PieceType.Bishop)));
            bcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Knight && p.Color == PieceColor.Black).Count(), 0, 2)).Select(p => new Piece(PieceColor.Black, PieceType.Knight)));
            bcap.AddRange(Enumerable.Range(0, Math.Clamp(1 - fpieces.Where(p => p.Type == PieceType.Queen && p.Color == PieceColor.Black).Count(), 0, 1)).Select(p => new Piece(PieceColor.Black, PieceType.Queen)));

            WhiteCaptured = wcap.ToArray();
            BlackCaptured = bcap.ToArray();
        }

        public override string ToString()
        {
            string sPieces = "";

            for (int i = 7; i >= 0; i--)
            {
                int emptyCount = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] is null)
                        emptyCount++;
                    else
                    {
                        if (emptyCount > 0)
                        {
                            sPieces += emptyCount;
                            emptyCount = 0;
                        }
                        sPieces += pieces[i, j].ToFenChar();
                    }
                }
                if (emptyCount > 0)
                    sPieces += emptyCount;
                if (i - 1 >= 0)
                    sPieces += "/";
            }

            string sCastles = "";

            if (CastleWK)
                sCastles += "K";
            if (CastleWQ)
                sCastles += "Q";
            if (CastleBK)
                sCastles += "k";
            if (CastleBQ)
                sCastles += "q";

            if (sCastles == "")
                sCastles = "-";

            string sEnPas;

            if (EnPassant.HasValue)
                sEnPas = EnPassant.ToString();
            else
                sEnPas = "-";

            return string.Join(' ', sPieces, Turn.AsChar, sCastles, sEnPas, HalfMoves, FullMoves);
        }
    }
}