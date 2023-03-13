namespace Chess;

#pragma warning disable CS1591

public delegate void CaptureEventHandler(object sender, CaptureEventArgs e);

public class CaptureEventArgs : ChessEventArgs
{
    /// <summary>
    /// Piece that has been captured
    /// </summary>
    public Piece CapturedPiece { get; }

    /// <summary>
    /// List of captured pieces where color == White
    /// </summary>
    public Piece[] WhiteCapturedPieces { get; set; }

    /// <summary>
    /// List of captured pieces where color == Black
    /// </summary>
    public Piece[] BlackCapturedPieces { get; set; }

    public CaptureEventArgs(Chessboard chessboard, Piece capturedPiece, Piece[] whiteCapturedPieces, Piece[] blackCapturedPieces) : base(chessboard)
    {
        CapturedPiece = capturedPiece;
        WhiteCapturedPieces = whiteCapturedPieces;
        BlackCapturedPieces = blackCapturedPieces;
    }
}