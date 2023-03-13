namespace Chess;

#pragma warning disable CS1591

public delegate void CheckedChangedEventHandler(object sender, CheckedChangedEventArgs e);

public class CheckedChangedEventArgs : ChessEventArgs
{
    /// <summary>
    /// Position of checked king
    /// </summary>
    public Position KingPosition { get; }

    /// <summary>
    /// Checked state
    /// </summary>
    public bool IsChecked { get; }

    public CheckedChangedEventArgs(Chessboard chessboard, Position kingPosition, bool isChecked) : base(chessboard)
    {
        KingPosition = kingPosition;
        IsChecked = isChecked;
    }
}