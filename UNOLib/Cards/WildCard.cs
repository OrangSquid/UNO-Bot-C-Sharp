namespace UNOLib.Cards;

/// <summary>
/// Wild Cards
/// </summary>
public class WildCard : ICard
{
    public const string ActualColor = "Wild";

    public CardColors Color { get; set; }
    public required WildCardSymbols Symbol { get; init; }

    public WildCard()
    {
        Color = CardColors.Red;
    }

    public bool CanBePlayed(ICard card) => card is WildCard || card.Color == Color;

    public override string ToString()
    {
        return ActualColor + " " + Symbol;
    }
}