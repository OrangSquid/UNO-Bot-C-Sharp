namespace UNOLib;

internal abstract class AbstractDrawStyle : IDrawStyle
{
    private static readonly Random rng = new();
    private Stack<ICard> _fullDeck;
    private readonly List<ICard> _playedCards;

    protected AbstractDrawStyle(List<ICard> fullDeck, int number_total_cards)
    {
        _fullDeck = new(fullDeck.OrderBy(a => rng.Next()).ToList());
        _playedCards = new List<ICard>(number_total_cards);
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
            _fullDeck = new(_playedCards.OrderBy(a => rng.Next()).ToList());
            _playedCards.Clear();
        }
        return card;
    }

    public abstract bool GameDraw(ref GameState state);
}
