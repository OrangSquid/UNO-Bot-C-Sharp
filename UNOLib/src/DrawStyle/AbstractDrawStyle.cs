using UNOLib.Cards;

namespace UNOLib.DrawStyle;

internal abstract class AbstractDrawStyle : IDrawStyle
{
    private static readonly Random Rng = new();
    private Stack<ICard> _fullDeck;
    private readonly List<ICard> _playedCards;

    protected AbstractDrawStyle(IEnumerable<ICard> fullDeck, int numberTotalCards)
    {
        _fullDeck = new Stack<ICard>(fullDeck.OrderBy(_ => Rng.Next()).ToList());
        _playedCards = new List<ICard>(numberTotalCards);
    }

    public void Push(ICard card)
    {
        if (card is not WildCard { Color: CardColors.None })
            return;
        _playedCards.Add(card);
    }

    public ICard Draw()
    {
        var card = _fullDeck.Pop();
        if (_fullDeck.Count == 0)
        {
            _fullDeck = new Stack<ICard>(_playedCards.OrderBy(_ => Rng.Next()).ToList());
            _playedCards.Clear();
        }
        return card;
    }

    public abstract bool GameDraw(ref GameState state);
}