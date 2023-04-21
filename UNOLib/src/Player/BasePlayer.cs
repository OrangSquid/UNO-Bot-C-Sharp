using System.Collections;
using UNOLib.Cards;
using UNOLib.Exceptions;

namespace UNOLib.Player;

public class BasePlayer : IPlayer
{
    private readonly SortedDictionary<string, Stack<ICard>> _deck = new(new CardComparer());
    private int _numPlusTwoCards;
    private int _numWildPlusFourCards;

    public int Id { get; }
    bool IPlayer.HasPlusTwoCards => _numPlusTwoCards > 0;
    bool IPlayer.HasWildPlusFourCards => _numWildPlusFourCards > 0;
    public int NumCards { get; private set; }

    protected internal BasePlayer(int id)
    {
        Id = id;
    }

    void IPlayer.AddCard(ICard card)
    {
        // Check if player has card and store it in cardsSameValue
        if (!_deck.TryGetValue(card.ToString(), out Stack<ICard>? cardsSameValue))
        {
            cardsSameValue = new Stack<ICard>();
            _deck.Add(card.ToString(), cardsSameValue);
        }
        cardsSameValue.Push(card);
        // Update _numPlusTwoCards, _numWildPlusFourCards and _numCards
        if (card is ColorCard { Symbol: ColorCardSymbols.PlusTwo })
        {
            _numPlusTwoCards++;
        }
        else if (card is WildCard { Symbol: WildCardSymbols.PlusFour })
        {
            _numWildPlusFourCards++;
        }
        NumCards++;
    }

    bool IPlayer.RemoveCard(string cardId)
    {
        // Check if player has card and store it in cardsSameValue
        if (!_deck.TryGetValue(cardId, out Stack<ICard>? cardsSameValue))
        {
            throw new PlayerDoesNotHaveCardException();
        }
        var card = cardsSameValue.Pop();
        // Clear stack of the same cards
        if (cardsSameValue.Count == 0)
        {
            _deck.Remove(cardId);
        }
        // Update _numPlusTwoCards, _numWildPlusFourCards and _numCards
        if (card is ColorCard { Symbol: ColorCardSymbols.PlusTwo })
        {
            _numPlusTwoCards--;
        }
        else if (card is WildCard { Symbol: WildCardSymbols.PlusFour })
        {
            _numWildPlusFourCards--;
        }
        NumCards--;
        return NumCards == 1;
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