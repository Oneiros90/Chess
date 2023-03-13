// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

public partial class Chessboard
{
    private readonly SynchronizationContext? context = SynchronizationContext.Current;

    /// <summary>
    ///     Raises when trying to make or validate move but after the move would have been made, king would have been checked
    /// </summary>
    public event CheckedChangedEventHandler OnInvalidMoveKingChecked = delegate { };

    /// <summary>
    ///     Raises when white king is (un)checked
    /// </summary>
    public event CheckedChangedEventHandler OnWhiteKingCheckedChanged = delegate { };

    /// <summary>
    ///     Raises when black king is (un)checked
    /// </summary>
    public event CheckedChangedEventHandler OnBlackKingCheckedChanged = delegate { };

    /// <summary>
    ///     Raises when user has to choose promotion action
    /// </summary>
    public event PromotionEventHandler OnPromotePawn = delegate { };

    /// <summary>
    ///     Raises when it's end of game
    /// </summary>
    public event EndgameEventHandler OnEndGame = delegate { };

    /// <summary>
    ///     Raises when any piece has been captured
    /// </summary>
    public event CaptureEventHandler OnCaptured = delegate { };

    private void OnWhiteKingCheckedChangedEvent(CheckedChangedEventArgs e)
    {
        if (context is not null)
        {
            context.Send(delegate { OnWhiteKingCheckedChanged(this, e); }, null);
        }
        else
        {
            OnWhiteKingCheckedChanged(this, e);
        }
    }

    private void OnBlackKingCheckedChangedEvent(CheckedChangedEventArgs e)
    {
        if (context is not null)
        {
            context.Send(delegate { OnBlackKingCheckedChanged(this, e); }, null);
        }
        else
        {
            OnBlackKingCheckedChanged(this, e);
        }
    }

    private void OnInvalidMoveKingCheckedEvent(CheckedChangedEventArgs e)
    {
        if (context is not null)
        {
            context.Send(delegate { OnInvalidMoveKingChecked(this, e); }, null);
        }
        else
        {
            OnInvalidMoveKingChecked(this, e);
        }
    }

    private void OnPromotePawnEvent(PromotionEventArgs e)
    {
        if (context is not null)
        {
            context.Send(delegate { OnPromotePawn(this, e); }, null);
        }
        else
        {
            OnPromotePawn(this, e);
        }
    }

    private void OnEndGameEvent()
    {
        if (context is not null)
        {
            context.Send(delegate { OnEndGame(this, new EndgameEventArgs(this, Endgame)); }, null);
        }
        else
        {
            OnEndGame(this, new EndgameEventArgs(this, Endgame));
        }
    }

    private void OnCapturedEvent(Piece piece)
    {
        if (context is not null)
        {
            context.Send(
                delegate { OnCaptured(this, new CaptureEventArgs(this, piece, CapturedWhite, CapturedBlack)); }, null);
        }
        else
        {
            OnCaptured(this, new CaptureEventArgs(this, piece, CapturedWhite, CapturedBlack));
        }
    }
}