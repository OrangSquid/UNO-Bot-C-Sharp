namespace UNOLib.Cards;

/// <summary>
/// Wild Cards
/// </summary>
public class WildCard : ICard
{
    public CardColors Color { get; set; }

    public WildCardSymbols Symbol { get; }
    public const string ActualColor = "Wild";

    public WildCard(CardColors color, WildCardSymbols symbol)
    {
        Color = color;
        Symbol = symbol;
    }

    public bool CanBePlayed(ICard card)
    {
        return card is WildCard || card.Color == Color;
    }

    public override string ToString()
    {
        return ActualColor + " " + Symbol.ToString();
    }
}