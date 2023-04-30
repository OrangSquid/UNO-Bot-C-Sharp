namespace UNOLib.Cards;

/// <summary>
/// Wild Cards
/// </summary>
public class WildCard : ICard
{
    public CardColors Color { get; }
    public WildCardSymbols Symbol { get; }

    protected List<WildCard>? ColoredWildCards;

    private readonly string _stringRepresentation;

    public WildCard(WildCardSymbols symbol) : this(CardColors.None, symbol)
    {
        InitializeColoredWildCards();
    }

    protected WildCard(CardColors color, WildCardSymbols symbol)
    {
        Color = color;
        Symbol = symbol;
        _stringRepresentation = color == CardColors.None ? "Wild " + Symbol : color + " Wild " + Symbol;
    }

    private void InitializeColoredWildCards()
    {
        ColoredWildCards = new List<WildCard>(4);
        foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            ColoredWildCards.Add(new WildCard(color, Symbol));
        }
    }

    public WildCard ChangeColor(string color)
    {
        return ColoredWildCards!.Find(card => card.Color == Enum.Parse<CardColors>(color))!;
    }

    public bool CanBePlayed(ICard card) => card is WildCard || card.Color == Color;

    public override string ToString()
    {
        return _stringRepresentation;
    }
}
