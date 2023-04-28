namespace UNOLib.Cards;

/// <summary>
/// Wild Cards
/// </summary>
public class WildCard : ICard
{
    public CardColors Color { get; set; }
    public required WildCardSymbols Symbol { get; init; }

    private readonly string _stringRepresentation;

    public WildCard()
    {
        Color = CardColors.Red;

        _stringRepresentation = "Wild " + Symbol;
    }

    public bool CanBePlayed(ICard card) => card is WildCard || card.Color == Color;

    public override string ToString()
    {
        return _stringRepresentation;
    }
}
