
namespace UNOLib
{
    internal class WildCard : ICard
    {
        public CardColors Color { get; set; }

        internal WildCardSymbols Symbol { get; init; }
        private const string ACTUAL_COLOR = "Wild";

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
            return string.Concat(ACTUAL_COLOR, Symbol.ToString());
        }
    }
}
