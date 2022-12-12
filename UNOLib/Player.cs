
using System.Collections;
using UNOLib.Exceptions;

namespace UNOLib
{
    internal class Player : IPlayer
    {
        public int Id { get; init; }
        private readonly SortedDictionary<string, Stack<ICard>> _deck;

        public Player(int id)
        {
            Id = id;
            _deck = new();
        }

        public bool HasCard(string cardId)
        {
            return _deck.ContainsKey(cardId);
        }

        public void AddCard(ICard card)
        {
            if(!_deck.TryGetValue(card.ToString(), out Stack<ICard>? cardsSameValue))
            {
                cardsSameValue = new Stack<ICard>();
                _deck.Add(card.ToString(), cardsSameValue);
            }
            cardsSameValue.Push(card);
        }

        public void RemoveCard(string cardId)
        {
            if (!_deck.TryGetValue(cardId, out Stack<ICard>? cardsSameValue))
            {
                throw new PlayerDoesNotHaveCardException();
            }
            cardsSameValue.Pop();
            if(cardsSameValue.Count == 0)
            {
                _deck.Remove(cardId);
            }
        }

        public ICard GetCard(string cardId)
        {
            if (!_deck.TryGetValue(cardId, out Stack<ICard>? cardsSameValue))
            {
                throw new PlayerDoesNotHaveCardException();
            }
            return cardsSameValue.Peek();
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
