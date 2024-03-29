﻿using System.Collections;
using UNOLib.Cards;

namespace UNOLib.Player;

/// <summary>
/// Enumerator that iterates over all the cards in a Player's Deck
/// </summary>
public class PlayerDeckEnumerator : IEnumerator<ICard>
{
    public ICard Current => _stackEnumerator.Current;

    object IEnumerator.Current => Current;

    private IEnumerator<ICard> _stackEnumerator;
    private SortedDictionary<string, Stack<ICard>>.Enumerator _dictionaryEnumerator;

    public PlayerDeckEnumerator(SortedDictionary<string, Stack<ICard>>.Enumerator enumerator)
    {
        _dictionaryEnumerator = enumerator;
        _dictionaryEnumerator.MoveNext();
        _stackEnumerator = _dictionaryEnumerator.Current.Value.GetEnumerator();
    }

    public void Dispose()
    {
        _stackEnumerator.Dispose();
        _dictionaryEnumerator.Dispose();
    }

    public bool MoveNext()
    {
        if (_stackEnumerator.MoveNext())
        {
            return true;
        }
        if (_dictionaryEnumerator.MoveNext())
        {
            _stackEnumerator.Dispose();
            _stackEnumerator = _dictionaryEnumerator.Current.Value.GetEnumerator();
            return MoveNext();
        }
        return false;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}