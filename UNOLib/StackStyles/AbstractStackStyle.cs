using UNOLib.DrawStyle;

namespace UNOLib.StackStyles;

internal abstract class AbstractStackStyle : IStackStyle
{
    private const int PLUS_TWO = 2;
    private const int PLUS_FOUR = 4;
    private IDrawStyle _drawStyle;

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
            switch (wildCard.Symbol)
            {
                case WildCardSymbols.PlusFour:
                    return PLUS_FOUR;
            }
        }
        else if (card is ColorCard colorCard)
        {
            switch (colorCard.Symbol)
            {
                case ColorCardSymbols.PlusTwo:
                    return PLUS_TWO;
            }
        }
        return 0;
    }

    public abstract bool ForcedDraw(ref GameState state, ICard card);
}
