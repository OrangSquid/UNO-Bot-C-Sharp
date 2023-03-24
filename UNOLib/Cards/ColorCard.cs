namespace UNOLib.Cards;

/// <summary>
/// Color Cards
/// </summary>
public class ColorCard : ICard
{
    public required CardColors Color { get; init; }
    public required ColorCardSymbols Symbol { get; init; }

    public bool CanBePlayed(ICard card) => card is WildCard || card.Color == Color || card is ColorCard cCard && cCard.Symbol == Symbol;

    public override string ToString()
    {
        string message = Color + " ";

        message += Symbol switch
    {
            ColorCardSymbols.Reverse or ColorCardSymbols.PlusTwo or ColorCardSymbols.Skip => Symbol.ToString(),
            _ => Enum.Format(typeof(ColorCardSymbols), Symbol, "d")
        };

        return message;
    }

    //public string ToURL()
    //{
    //    string message = Color.ToString().ToLower() + "%20";

    //    if (Symbol.Equals(ColorCardSymbols.Reverse) || Symbol.Equals(ColorCardSymbols.PlusTwo) || Symbol.Equals(ColorCardSymbols.Skip))
    //        message += Symbol.ToString().ToLower();
    //    else
    //        message += Enum.Format(typeof(ColorCardSymbols), Symbol, "d");
    //    message += ".png";

    //    return message;
    //}
}
