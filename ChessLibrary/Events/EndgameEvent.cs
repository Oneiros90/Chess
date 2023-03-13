namespace Chess;

#pragma warning disable CS1591

public delegate void EndgameEventHandler(object sender, EndgameEventArgs e);

public class EndgameEventArgs : ChessEventArgs
{
    /// <summary>
    /// Endgame additional info
    /// </summary>
    public EndgameInfo? EndgameInfo { get; }

    public EndgameEventArgs(Chessboard chessboard, EndgameInfo? endgameInfo) : base(chessboard)
    {
        EndgameInfo = endgameInfo;
    }
}