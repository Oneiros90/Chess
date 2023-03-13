namespace Chess;

#pragma warning disable CS1591

public enum EndgameType : byte
{
    Checkmate,
    Resigned,
    Timeout,
    Stalemate,
    DrawDeclared,
    InsufficientMaterial,
    FiftyMoveRule,
    Repetition,
}