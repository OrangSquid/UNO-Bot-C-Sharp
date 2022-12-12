
namespace UNOLib
{
    internal class Player : IPlayer
    {
        public int Id { get; init; }
        private readonly SortedDictionary<string, ICard> _deck;

        public Player(int id)
        {
            Id = id;
            _deck = new SortedDictionary<string, ICard>();
        }

        public void AddCard(ICard card) 
        {
            _deck.Add(card.ToString(), card);
        }
    }
}
