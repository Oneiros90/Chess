// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

/// <summary>
/// https://www.chessprogramming.org/Fifty-move_Rule
/// </summary>
internal class FiftyMoveRule : EndgameRule
{
    private const int MAX_HALF_MOVE_CLOCK = 100;

    public FiftyMoveRule(Chessboard board) : base(board) { }

    internal override EndgameType Type => EndgameType.FiftyMoveRule;

    internal override bool IsEndGame()
    {
        return board.GetHalfMovesCount() >= MAX_HALF_MOVE_CLOCK;
    }
}

