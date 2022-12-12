
namespace UNOLib
{
    internal class WildCard : ICard
    {
        public CardColors Color { get; protected set; }
        public WildCardSymbols Symbol { get; init; }

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
            return String.Concat(Color.ToString(), Symbol.ToString());
        }
    }
}
