// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chess;

/// <summary>
/// Smart enum for Piece type in chess game
/// </summary>
public class PieceType : SmartEnum<PieceType>
{
    public static readonly PieceType Pawn = new("Pawn", 1, 'p');
    public static readonly PieceType Rook = new("Rook", 2, 'r');
    public static readonly PieceType Knight = new("Knight", 3, 'n');
    public static readonly PieceType Bishop = new("Bishop", 4, 'b');
    public static readonly PieceType Queen = new("Queen", 5, 'q');
    public static readonly PieceType King = new("King", 6, 'k');

    /// <summary>
    /// Pawn => 'p'<br/>
    /// Rook => 'r'<br/>
    /// Knight => 'n'<br/>
    /// Bishop => 'b'<br/>
    /// Queen => 'q'<br/>
    /// King => 'k'<br/>
    /// </summary>
    public char AsChar { get; }

    private PieceType(string name, int value, char asChar) : base(name, value)
    {
        AsChar = asChar;
    }

    /// <summary>
    /// PieceType object from char<br/>
    /// 'p' => Pawn<br/>
    /// 'r' => Rook<br/>
    /// 'n' => Knight<br/>
    /// 'b' => Bishop<br/>
    /// 'q' => Queen<br/>
    /// 'k' => King<br/>
    /// </summary>
    public static PieceType FromChar(char type)
    {
        return char.ToLower(type) switch
        {
            'p' => Pawn,
            'r' => Rook,
            'n' => Knight,
            'b' => Bishop,
            'q' => Queen,
            'k' => King,
            _ => throw new ArgumentException(null, nameof(type), nameof(PieceType.FromChar)),
        };
    }
}
