using System.Drawing;

namespace UNOLib.Cards;

/// <summary>
/// Color Cards
/// </summary>
public class ColorCard : ICard
{
    public required CardColors Color { get; init; }
    public required ColorCardSymbols Symbol { get; init; }

    private readonly string _stringRepresentation;

    public ColorCard() {

        _stringRepresentation = Color + " ";

        _stringRepresentation += Symbol switch
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
