namespace Chess;

#pragma warning disable CS1591

public class SanTooAmbiguousException : ChessException
{
    public string SanMove { get; set; }
    public Move[] Moves { get; }
    public SanTooAmbiguousException(Chessboard board, string san, Move[] moves)
        : this(board, $"Given SAN move: {san} is too ambiguous between moves: {string.Join(", ", moves.Select(m => m.ToString()))}", san, moves) { }
    public SanTooAmbiguousException(Chessboard board, string message, string san, Move[] moves) : base(board, message)
    {
        SanMove = san;
        Moves = moves;
    }
}