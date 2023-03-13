// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

public partial class Chessboard
{
    /// <summary>
    /// Tries to load ChessBoard from Portable Game Notation<br/>
    /// ex.:<br/>
    /// [White "Milan1905"]<br/>
    /// [Black "Geras1mleo"]<br/>
    /// [Result "1-0"]<br/>
    /// [WhiteElo "1006"]<br/>
    /// [BlackElo "626"]<br/>
    /// [Termination "Milan1905 won by resignation"]<br/>
    /// <br/>
    /// 1. e4 e5 2. Nf3 Nf6 3...
    /// </summary>
    /// <param name="pgn">PGN string to load</param>
    /// <param name="board">Result with loaded board</param>
    /// <param name="autoEndgameRules">Automatic draw/endgame rules that will be used to check for endgame</param>
    /// <returns>Whether load is succeeded</returns>
    public static bool TryLoadFromPgn(string pgn, [NotNullWhen(true)] out Chessboard? board, AutoEndgameRules autoEndgameRules = AutoEndgameRules.None)
    {
        return PgnBuilder.TryLoad(pgn, out board, autoEndgameRules).succeeded;
    }

    /// <summary>
    /// Loads ChessBoard from Portable Game Notation<br/>
    /// ex.:<br/>
    /// [White "Milan1905"]<br/>
    /// [Black "Geras1mleo"]<br/>
    /// [Result "1-0"]<br/>
    /// [WhiteElo "1006"]<br/>
    /// [BlackElo "626"]<br/>
    /// [Termination "Milan1905 won by resignation"]<br/>
    /// <br/>
    /// 1. e4 e5 2. Nf3 Nf6 3...
    /// </summary>
    /// <param name="pgn">PGN string to load</param>
    /// <param name="autoEndgameRules">Automatic draw/endgame rules that will be used to check for endgame</param>
    /// <returns>ChessBoard with according positions</returns>
    /// <exception cref="ArgumentException">Given FEN string didn't match the Regex pattern (if PGN contains FEN in header)</exception>
    /// <exception cref="ArgumentException">Given San-notated move didn't match the Regex pattern</exception>
    /// <exception cref="SanNotFoundException">Given San-notated move is not valid for current board positions</exception>
    /// <exception cref="SanTooAmbiguousException">Given San-notated move is too ambiguous between multiple moves</exception>
    public static Chessboard LoadFromPgn(string pgn, AutoEndgameRules autoEndgameRules = AutoEndgameRules.None)
    {
        var (succeeded, exception) = PgnBuilder.TryLoad(pgn, out var board, autoEndgameRules);

        if (!succeeded && exception is not null)
            throw exception;

        return board!;
    }
}