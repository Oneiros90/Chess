#pragma warning disable CS1591

namespace Chess;

public class ChessException : Exception
{
    public Chessboard? Board { get; }
    protected ChessException(Chessboard? board, string message) : base(message) => Board = board;
}