
using System.Collections;
using UNOLib.Exceptions;

namespace UNOLib;

internal class Player : IPlayer
{
    private readonly SortedDictionary<string, Stack<ICard>> _deck;
    private int _numPlusTwoCards;
    private int _numWildPlusFourCards;

    public int Id { get; }

    public bool HasPlusTwoCards => _numPlusTwoCards > 0;

    public bool HasWildPlusFourCards => _numWildPlusFourCards > 0;

    public int NumCards { get; private set; }

    public Player(int id)
    {
        Id = id;
        NumCards = 0;
        _numPlusTwoCards = 0;
        _numWildPlusFourCards = 0;
        _deck = new(new CardComparer());
    }

    public void AddCard(ICard card)
    {
        // Check if player has card and store it in cardsSameValue
        if (!_deck.TryGetValue(card.ToString(), out Stack<ICard>? cardsSameValue))
        {
            cardsSameValue = new Stack<ICard>();
            _deck.Add(card.ToString(), cardsSameValue);
        }
        cardsSameValue.Push(card);
        // Update _numPlusTwoCards, _numWildPlusFourCards and _numCards
        if (card is ColorCard cCard && cCard.Symbol == ColorCardSymbols.PlusTwo)
        {
            _numPlusTwoCards++;
        }
        else if (card is WildCard wCard && wCard.Symbol == WildCardSymbols.PlusFour)
        {
            _numWildPlusFourCards++;
        }
        NumCards++;
    }

    public void RemoveCard(string cardId)
    {
        // Check if player has card and store it in cardsSameValue
        if (!_deck.TryGetValue(cardId, out Stack<ICard>? cardsSameValue))
        {
            throw new PlayerDoesNotHaveCardException();
        }
        ICard card = cardsSameValue.Pop();
        // Clear stack of the same cards
        if (cardsSameValue.Count == 0)
        {
            _deck.Remove(cardId);
        }
        // Update _numPlusTwoCards, _numWildPlusFourCards and _numCards
        if (card is ColorCard cCard && cCard.Symbol == ColorCardSymbols.PlusTwo)
        {
            _numPlusTwoCards--;
        }
        else if (card is WildCard wCard && wCard.Symbol == WildCardSymbols.PlusFour)
        {
            _numWildPlusFourCards--;
        }
        NumCards--;
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