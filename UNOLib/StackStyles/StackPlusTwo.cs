using UNOLib.Cards;
using UNOLib.DrawStyle;

namespace UNOLib.StackStyles;

internal class StackPlusTwo : AbstractStackStyle
{
    public StackPlusTwo(IDrawStyle drawStyle) : base(drawStyle) { }

    public override bool ForcedDraw(ref GameState state, ICard card)
    {
        state.CardsDrawn += HowManyCardsToDraw(card);
        if (card is ColorCard { Symbol: ColorCardSymbols.PlusTwo } && state.CurrentPlayer.HasPlusTwoCards)
        {
            state.StackPlusTwo = true;
            return false;
        }

        FinalDraw(ref state, state.CardsDrawn);
        return true;
    }
}
