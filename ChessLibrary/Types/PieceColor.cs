namespace Chess;

#pragma warning disable CS1591
/// <summary>
/// Smart enum for Piece color in chess game
/// </summary>
public class PieceColor : SmartEnum<PieceColor>
{
    public static readonly PieceColor White = new("White", 1, 'w');
    public static readonly PieceColor Black = new("Black", 2, 'b');

    /// <summary>
    /// White => 'w'<br/>
    /// Black => 'b'<br/>
    /// </summary>
    public char AsChar { get; }

    /// <summary>
    /// Opposite color <br/>
    /// White => Black<br/>
    /// Black => White<br/>
    /// </summary>
    public PieceColor OppositeColor()
    {
        return Value switch
        {
            1 => Black,
            2 => White,
            _ => throw new ArgumentException(null, nameof(Value), nameof(PieceColor.OppositeColor)),
        };
    }

    private PieceColor(string name, int value, char asChar) : base(name, value)
    {
        AsChar = asChar;
    }

    /// <summary>
    /// PieceColor object from char<br/>
    /// 'w' => White<br/>
    /// 'b' => Black<br/>
    /// </summary>
    public static PieceColor FromChar(char color)
    {
        return char.ToLower(color) switch
        {
            'w' => White,
            'b' => Black,
            _ => throw new ArgumentException(null, nameof(color), nameof(PieceColor.FromChar)),
        };
    }
}