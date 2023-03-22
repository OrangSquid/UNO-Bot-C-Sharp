using UNOLib.Cards;
using UNOLib.DrawStyle;

namespace UNOLib.StackStyles;

internal abstract class AbstractStackStyle : IStackStyle
{
    private const int PlusTwo = 2;
    private const int PlusFour = 4;
    private readonly IDrawStyle _drawStyle;

    protected AbstractStackStyle(IDrawStyle drawStyle)
    {
        _drawStyle = drawStyle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cardsToDraw"></param>
    protected void FinalDraw(ref GameState state, int cardsToDraw)
    {
        for (int i = 0; i < cardsToDraw; i++)
            state.CurrentPlayer.AddCard(_drawStyle.Draw());
        state.CardsDrawn = cardsToDraw;
        state.WhoDrewCards = state.CurrentPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    protected int HowManyCardsToDraw(ICard card)
    {
        if (card is WildCard wildCard)
        {
            if (wildCard.Symbol == WildCardSymbols.PlusFour) return PlusFour;
        }
        else if (card is ColorCard { Symbol: ColorCardSymbols.PlusTwo }) return PlusTwo;

        return 0;
    }

    public abstract bool ForcedDraw(ref GameState state, ICard card);
}
