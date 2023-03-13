namespace Chess;

#pragma warning disable CS1591

public class ArgumentException : ChessException
{
    public ArgumentException(Chessboard? board, string argument, string method)
        : this(board, $"An argument: {argument} in method: {method} is not valid...") { }
    public ArgumentException(Chessboard? board, string message) : base(board, message) { }
}