// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class EndgameProvider
{
    private Chessboard board;

    public EndgameProvider(Chessboard board)
    {
        this.board = board;
    }

    public EndgameInfo? GetEndGameInfo()
    {
        EndgameInfo? endgameInfo = null;

        if (board.moveIndex >= 0
         && board.executedMoves[board.moveIndex].IsMate)
        {
            if (board.executedMoves[board.moveIndex].IsCheck)
                endgameInfo = new EndgameInfo(EndgameType.Checkmate, board.Turn.OppositeColor());
            else
                endgameInfo = new EndgameInfo(EndgameType.Stalemate, null);
        }
        else if (board.LoadedFromFen)
        {
            var whiteHasMoves = Chessboard.PlayerHasMoves(PieceColor.White, board);
            var blackHasMoves = Chessboard.PlayerHasMoves(PieceColor.Black, board);

            if (!whiteHasMoves && board.WhiteKingChecked)
                endgameInfo = new EndgameInfo(EndgameType.Checkmate, PieceColor.Black);

            else if (!blackHasMoves && board.BlackKingChecked)
                endgameInfo = new EndgameInfo(EndgameType.Checkmate, PieceColor.White);

            else if (!whiteHasMoves || !blackHasMoves)
                endgameInfo = new EndgameInfo(EndgameType.Stalemate, null);
        }

        if (endgameInfo is null)
        {
            endgameInfo = ResolveDrawRules(board.AutoEndgameRules);
        }

        return endgameInfo;
    }

    private EndgameInfo? ResolveDrawRules(AutoEndgameRules autoEndgameRules)
    {
        EndgameInfo? endgameInfo = null;

        var rules = new List<EndgameRule>();

        if ((autoEndgameRules & AutoEndgameRules.InsufficientMaterial) == AutoEndgameRules.InsufficientMaterial)
            rules.Add(new InsufficientMaterialRule(board));

        if ((autoEndgameRules & AutoEndgameRules.Repetition) == AutoEndgameRules.Repetition)
            rules.Add(new RepetitionRule(board));

        if ((autoEndgameRules & AutoEndgameRules.FiftyMoveRule) == AutoEndgameRules.FiftyMoveRule)
            rules.Add(new FiftyMoveRule(board));

        for (int i = 0; i < rules.Count && endgameInfo is null; i++)
        {
            if (rules[i].IsEndGame())
            {
                endgameInfo = new EndgameInfo(rules[i].Type, null);
            }
        }

        return endgameInfo;
    }
}

