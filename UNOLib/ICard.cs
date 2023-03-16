
namespace UNOLib;

/// <summary>
/// Enumeration with the card colors
/// </summary>
public enum CardColors
{
    Red,
    Green,
    Yellow,
    Blue
}

/// <summary>
/// Enumeration with the Color Card's symbols
/// </summary>
internal enum ColorCardSymbols
{
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Skip,
    Reverse,
    PlusTwo
}

/// <summary>
/// Enumeration with the Wild Card's symbols
/// </summary>
internal enum WildCardSymbols
{
    Simple,
    PlusFour
}

/// <summary>
/// Interface for the types of cards that manages it's operations
/// </summary>
public interface ICard
{
    internal CardColors Color { get; }

    /// <summary>
    /// Checks if the given card can be played on top of the card on the table
    /// </summary>
    /// <param name="card">Card to be played</param>
    /// <returns>True if it can be played and false otherwise</returns>
    internal bool CanBePlayed(ICard card);
    public string ToString();
}
