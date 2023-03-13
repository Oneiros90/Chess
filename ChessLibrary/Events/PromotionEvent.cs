namespace Chess;

#pragma warning disable CS1591

public delegate void PromotionEventHandler(object sender, PromotionEventArgs e);

public class PromotionEventArgs : ChessEventArgs
{
    /// <summary>
    /// Specified by user promotion result
    /// </summary>
    public PromotionType PromotionResult { get; set; } = PromotionType.Default;

    public PromotionEventArgs(Chessboard chessboard) : base(chessboard) { }
}