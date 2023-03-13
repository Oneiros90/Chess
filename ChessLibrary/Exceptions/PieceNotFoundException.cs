namespace Chess;

#pragma warning disable CS1591

public class PieceNotFoundException : ChessException
{
    public Position Position { get; }
    public PieceNotFoundException(Chessboard board, Position position)
        : this(board, $"Piece on given position: {position} has been not found in current chess board", position) { }
    public PieceNotFoundException(Chessboard board, string message, Position position) : base(board, message) => Position = position;
}