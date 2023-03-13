namespace Chess;

#pragma warning disable CS1591

public abstract class ChessEventArgs : EventArgs
{
    public Chessboard Chessboard { get; }

    protected ChessEventArgs(Chessboard chessboard)
    {
        Chessboard = chessboard;
    }
}