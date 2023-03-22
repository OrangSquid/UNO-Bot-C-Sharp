using UNOLib.DrawStyle;
using UNOLib.StackStyles;

namespace UNOLib;

public class GameSystemFactory
{
    private const int NumberCards = 108;
    private const int NumberOfZeros = 1;
    private const int NumberOfEachColorCard = 2;
    private const int NumberOfEachWildCard = 4;

    private static readonly Dictionary<string, ICard> AllCardsDict;
    private static readonly List<ICard> AllCards;
    private readonly int _nPlayers;

    public required bool DrawUntilPlayableCard { get; init; }
    public required bool StackPlusTwo { get; init; }
    public required bool MustPlay { get; init; }

    static GameSystemFactory()
    {
        AllCardsDict = new(NumberCards);
        AllCards = new(NumberCards);
        foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            foreach (ColorCardSymbols symbol in Enum.GetValuesAsUnderlyingType<ColorCardSymbols>())
            {
                ICard card = new ColorCard(color, symbol);
                AllCardsDict.Add(card.ToString(), card);
                int i = symbol == ColorCardSymbols.Zero ? NumberOfZeros : NumberOfEachColorCard;
                for (; i > 0; i--)
                {
                    AllCards.Add(card);
                }
            }
        }
        foreach (WildCardSymbols symbol in Enum.GetValuesAsUnderlyingType<WildCardSymbols>())
        {
            // CardColors.RED is simply used as a placeholder
            ICard card = new WildCard(CardColors.Red, symbol);
            AllCardsDict.Add(card.ToString(), card);
            for (int i = 0; i < NumberOfEachWildCard; i++)
            {
                AllCards.Add(card);
            }
        }
    }

    public GameSystemFactory(int nPlayers)
    {
        _nPlayers = nPlayers;
    }

    public IGameSystem Build()
    {
        IDrawStyle drawStyle;
        if (DrawUntilPlayableCard)
        {
            drawStyle = new DrawUntilFound(AllCards, NumberCards);
        }
        else
        {
            drawStyle = new DrawSingle(AllCards, NumberCards);
        }

        IStackStyle stackStyle;
        if (StackPlusTwo)
        {
            stackStyle = new StackPlusTwo(drawStyle);
        }
        else
        {
            stackStyle = new NoStack(drawStyle);
        }

        return new GameSystem(_nPlayers, AllCardsDict, drawStyle, MustPlay, stackStyle);
    }
}
