namespace Chess;

#pragma warning disable CS1591

public class InvalidMoveException : ChessException
{
    public Move Move { get; }
    public InvalidMoveException(Chessboard board, Move move)
        : this(board, $"Given move: {move} is invalid for current pieces positions.", move) { }
    public InvalidMoveException(Chessboard board, string message, Move move) : base(board, message) => Move = move;
}