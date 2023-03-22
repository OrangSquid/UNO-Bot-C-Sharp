namespace UNOLib;

/// <summary>
/// Color Cards
/// </summary>
internal class ColorCard : ICard
{
    public CardColors Color { get; }
    public ColorCardSymbols Symbol { get; init; }

    public ColorCard(CardColors color, ColorCardSymbols symbol)
    {
        Color = color;
        Symbol = symbol;
    }

    public bool CanBePlayed(ICard card)
    {
        return card is WildCard || card.Color == Color || card is ColorCard cCard && cCard.Symbol == Symbol;
    }

    public override string ToString()
    {
        string message = Color.ToString() + " ";

        if (Symbol.Equals(ColorCardSymbols.Reverse) || Symbol.Equals(ColorCardSymbols.PlusTwo) || Symbol.Equals(ColorCardSymbols.Skip))
            message += Symbol.ToString();
        else
            message += Enum.Format(typeof(ColorCardSymbols), Symbol, "d");

        return message;
    }
}