
using System.Collections;
using UNOLib.Exceptions;

namespace UNOLib
{
    internal class Player : IPlayer
    {
        private readonly SortedDictionary<string, Stack<ICard>> _deck;
        private int _numCards;
        public int Id { get; init; }

        public int NumCards
        {
            get
            {
                return _numCards;
            }
        }

        public Player(int id)
        {
            Id = id;
            _numCards = 0;
            _deck = new();
        }

        public bool HasCard(string cardId)
        {
            return _deck.ContainsKey(cardId);
        }

        public void AddCard(ICard card)
        {
            if (!_deck.TryGetValue(card.ToString(), out Stack<ICard>? cardsSameValue))
            {
                cardsSameValue = new Stack<ICard>();
                _deck.Add(card.ToString(), cardsSameValue);
            }
            cardsSameValue.Push(card);
            _numCards++;
        }

        public void RemoveCard(string cardId)
        {
            if (!_deck.TryGetValue(cardId, out Stack<ICard>? cardsSameValue))
            {
                throw new PlayerDoesNotHaveCardException();
            }
            _ = cardsSameValue.Pop();
            _numCards--;
            if (cardsSameValue.Count == 0)
            {
                _ = _deck.Remove(cardId);
            }
        }

        public ICard GetCard(string cardId)
        {
            return !_deck.TryGetValue(cardId, out Stack<ICard>? cardsSameValue)
                ? throw new PlayerDoesNotHaveCardException()
                : cardsSameValue.Peek();
        }

        public IEnumerator<ICard> GetEnumerator()
        {
            return new PlayerDeckEnumerator(_deck.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
