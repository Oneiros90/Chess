namespace Chess;

#pragma warning disable CS1591

[Flags]
public enum AutoEndgameRules : byte
{
    None = 0,
    InsufficientMaterial = 1,
    Repetition = 2,
    FiftyMoveRule = 4,
    All = 7
}