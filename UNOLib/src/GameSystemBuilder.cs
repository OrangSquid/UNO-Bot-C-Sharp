using UNOLib.Cards;
using UNOLib.DrawStyle;
using UNOLib.Player;
using UNOLib.StackStyles;

namespace UNOLib;

public class GameSystemBuilder
{
    private const int NumberCards = 108;
    protected const int NumberOfZeros = 1;
    protected const int NumberOfEachColorCard = 2;
    protected const int NumberOfEachWildCard = 4;
    private const int MaxPlayers = 6;

    protected static readonly Dictionary<string, ICard> AllCardsDict;
    protected static readonly List<ICard> AllCards;
    protected readonly List<IPlayer> PlayersByOrder;

    protected IDrawStyle? DrawStyle;
    protected IStackStyle? StackStyle;
    protected bool MustPlay;
    protected bool JumpIn;
    protected int UnoPenalty;

    static GameSystemBuilder()
    {
        AllCardsDict = new Dictionary<string, ICard>(NumberCards);
        AllCards = new List<ICard>(NumberCards);
        foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            if(color == CardColors.None) continue;

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
            ICard card = new WildCard(symbol);
            AllCardsDict.Add(card.ToString(), card);
            for (var i = 0; i < NumberOfEachWildCard; i++)
            {
                AllCards.Add(card);
            }
        }
    }

    public GameSystemBuilder()
    {
        PlayersByOrder = new List<IPlayer>(MaxPlayers);
    }

    public GameSystemBuilder CreatePlayers(int nPlayers)
    {
        for (var i = 0; i < nPlayers; i++)
        {
            PlayersByOrder.Add(new BasePlayer(i));
        }

        return this;
    }

    public GameSystemBuilder WithDrawUntilPlayable(bool drawUntilPlayable)
    {
        if (drawUntilPlayable)
        {
            DrawStyle = new DrawUntilFound(AllCards, NumberCards);
        }
        else
        {
            DrawStyle = new DrawSingle(AllCards, NumberCards);
        }
        return this;
    }

    public GameSystemBuilder WithStackPlusTwo(bool stackPlusTwo)
    {
        if (stackPlusTwo)
        {
            StackStyle = new StackPlusTwo(DrawStyle!);
        }
        else
        {
            StackStyle = new NoStack(DrawStyle!);
        }
        return this;
    }

    public GameSystemBuilder WithMustPlay(bool mustPlay)
    {
        MustPlay = mustPlay;
        return this;
    }

    public GameSystemBuilder WithJumpIn(bool jumpIn)
    {
        JumpIn = jumpIn;
        return this;
    }

    public GameSystemBuilder WithUnoPenalty(int unoPenalty)
    {
        UnoPenalty = unoPenalty;
        return this;
    }

    public virtual IGameSystem Build()
    {
        return new GameSystem(PlayersByOrder, AllCardsDict, DrawStyle!, MustPlay, StackStyle!, JumpIn, UnoPenalty);
    }
}