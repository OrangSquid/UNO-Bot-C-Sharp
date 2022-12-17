
namespace UNOLib
{
   public enum CardColors
    {
        Red,
        Green,
        Yellow,
        Blue
    }

    internal enum ColorCardSymbols
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Skip,
        Reverse,
        PlusTwo
    }

    internal enum WildCardSymbols
    {
        Simple,
        PlusFour
    }

    public interface ICard
    {
        internal CardColors Color { get; }
        internal bool CanBePlayed(ICard card);
        public string ToString();
    }
}
