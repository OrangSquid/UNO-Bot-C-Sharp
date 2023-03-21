namespace UNOLib;

public class GameSystemFactory
{
    private const int NUMBER_CARDS = 108;
    private const int NUMBER_OF_ZEROS = 1;
    private const int NUMBER_OF_EACH_COLOR_CARD = 2;
    private const int NUMBER_OF_EACH_WILD_CARD = 4;

    private static readonly Dictionary<string, ICard> _allCardsDict;
    private static readonly List<ICard> _allCards;
    private readonly int _nPlayers;

    public bool DrawUntilPlayableCard { get; init; }

    static GameSystemFactory()
    {
        _allCardsDict = new(NUMBER_CARDS);
        _allCards = new(NUMBER_CARDS);
        foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            foreach (ColorCardSymbols symbol in Enum.GetValuesAsUnderlyingType<ColorCardSymbols>())
            {
                ICard card = new ColorCard(color, symbol);
                _allCardsDict.Add(card.ToString(), card);
                int i = symbol == ColorCardSymbols.Zero ? NUMBER_OF_ZEROS : NUMBER_OF_EACH_COLOR_CARD;
                for (; i > 0; i--)
                {
                    _allCards.Add(card);
                }
            }
        }
        foreach (WildCardSymbols symbol in Enum.GetValuesAsUnderlyingType<WildCardSymbols>())
        {
            // CardColors.RED is simply used as a placeholder
            ICard card = new WildCard(CardColors.Red, symbol);
            _allCardsDict.Add(card.ToString(), card);
            for (int i = 0; i < NUMBER_OF_EACH_WILD_CARD; i++)
            {
                _allCards.Add(card);
            }
        }
    }

    public GameSystemFactory(int nPlayers)
    {
        _nPlayers = nPlayers;
    }

    public GameSystem Build()
    {
        IDrawStyle drawStyle;
        if (DrawUntilPlayableCard)
        {
            drawStyle = new DrawUntilFound(_allCards, NUMBER_CARDS);
        }
        else
        {
            drawStyle = new DrawSingle(_allCards, NUMBER_CARDS);
        }

        return new GameSystem(_nPlayers, _allCardsDict, drawStyle);
    }
}
