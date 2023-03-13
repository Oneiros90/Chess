namespace Chess;

#pragma warning disable CS1591

public class GameEndedException : ChessException
{
    public EndgameInfo EndgameInfo { get; }
    public GameEndedException(Chessboard board, EndgameInfo endgameInfo)
        : this(board, "This game is already ended.", endgameInfo) { }
    public GameEndedException(Chessboard board, string message, EndgameInfo endgameInfo) : base(board, message) => EndgameInfo = endgameInfo;
}