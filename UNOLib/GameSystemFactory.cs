using UNOLib.Cards;
using UNOLib.DrawStyle;
using UNOLib.Player;
using UNOLib.StackStyles;

namespace UNOLib;

public class GameSystemFactory
{
    protected const int NumberCards = 108;
    protected const int NumberOfZeros = 1;
    protected const int NumberOfEachColorCard = 2;
    protected const int NumberOfEachWildCard = 4;

    protected static Dictionary<string, ICard> AllCardsDict;
    protected static List<ICard> AllCards;
    protected readonly List<IPlayer> PlayersByOrder;

    public required bool DrawUntilPlayableCard { get; init; }
    public required bool StackPlusTwo { get; init; }
    public required bool MustPlay { get; init; }
    public required bool JumpIn { get; init; }
    public required int UnoPenalty { get; init; }

    static GameSystemFactory()
    {
        AllCardsDict = new Dictionary<string, ICard>(NumberCards);
        AllCards = new List<ICard>(NumberCards);
        foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            foreach (ColorCardSymbols symbol in Enum.GetValuesAsUnderlyingType<ColorCardSymbols>())
            {
                ICard card = new ColorCard()
                {
                    Color = color,
                    Symbol = symbol
                };
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
            ICard card = new WildCard
            {
                Symbol = symbol
            };
            AllCardsDict.Add(card.ToString(), card);
            for (var i = 0; i < NumberOfEachWildCard; i++)
            {
                AllCards.Add(card);
            }
        }
    }
    
    public GameSystemFactory(int nPlayers)
    {
        PlayersByOrder = new List<IPlayer>(nPlayers);
    }

    public void CreatePlayers()
    {
        for (var i = 0; i < PlayersByOrder.Capacity; i++)
        {
            PlayersByOrder.Add(new BasePlayer(i));
        }
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

        return new GameSystem(PlayersByOrder, AllCardsDict, drawStyle, MustPlay, stackStyle, JumpIn, UnoPenalty);
    }
}