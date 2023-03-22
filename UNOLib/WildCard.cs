
namespace UNOLib;

/// <summary>
/// Wild Cards
/// </summary>
internal class WildCard : ICard
{
    public CardColors Color { get; set; }

    internal WildCardSymbols Symbol { get; init; }
    private const string ActualColor = "Wild";

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