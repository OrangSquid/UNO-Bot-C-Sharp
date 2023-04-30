namespace UNOLib.Cards;

/// <summary>
/// Color Cards
/// </summary>
public class ColorCard : ICard
{
    public CardColors Color { get; }
    public ColorCardSymbols Symbol { get; }

    private readonly string _stringRepresentation;

    public ColorCard(CardColors color, ColorCardSymbols symbol)
    {
        Color = color;
        Symbol = symbol;

        _stringRepresentation =
            Color +
            " " +
            Symbol switch
            {
                ColorCardSymbols.Reverse or ColorCardSymbols.PlusTwo or ColorCardSymbols.Skip => Symbol.ToString(),
                _ => Enum.Format(typeof(ColorCardSymbols), Symbol, "d")
            };
    }

    public bool CanBePlayed(ICard card) => card is WildCard || card.Color == Color || card is ColorCard cCard && cCard.Symbol == Symbol;

    public override string ToString()
    {
        return _stringRepresentation;
    }
}
