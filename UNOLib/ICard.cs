
namespace UNOLib
{
    internal enum CardColors
    {
        RED, 
        GREEN,
        YELLOW, 
        BLUE
    }

    internal enum ColorCardSymbols
    {
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        SKIP,
        REVERSE,
        PLUS_TWO
    }

    internal enum WildCardSymbols {
        SIMPLE,
        PLUS_FOUR
    }

    internal interface ICard
    {
        public CardColors Color { get; }
        public bool CanBePlayed(ICard card);
    }
}
