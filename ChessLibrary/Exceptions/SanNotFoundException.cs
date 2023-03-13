namespace Chess;

#pragma warning disable CS1591

public class SanNotFoundException : ChessException
{
    public string SanMove { get; set; }
    public SanNotFoundException(Chessboard board, string san)
        : this(board, $"Given SAN move: {san} has been not found with current board positions.", san) { }
    public SanNotFoundException(Chessboard board, string message, string san) : base(board, message) => SanMove = san;
}