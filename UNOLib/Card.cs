
namespace UNOLib
{
    internal abstract class Card : ICard
    {

        public CardColors Color { get; init; }

        public abstract bool CanBePlayed(ICard card);
    }
}
