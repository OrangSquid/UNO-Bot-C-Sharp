namespace UNOLib.DrawStyle;

internal abstract class AbstractDrawStyle : IDrawStyle
{
    private static readonly Random Rng = new();
    private Stack<ICard> _fullDeck;
    private readonly List<ICard> _playedCards;

    protected AbstractDrawStyle(List<ICard> fullDeck, int numberTotalCards)
    {
        _fullDeck = new(fullDeck.OrderBy(a => Rng.Next()).ToList());
        _playedCards = new List<ICard>(numberTotalCards);
    }

    public void Push(ICard card)
    {
        _playedCards.Add(card);
    }

    public ICard Draw()
    {
        ICard card = _fullDeck.Pop();
        if (_fullDeck.Count == 0)
        {
            _fullDeck = new(_playedCards.OrderBy(a => Rng.Next()).ToList());
            _playedCards.Clear();
        }
        return card;
    }

    public abstract bool GameDraw(ref GameState state);
}