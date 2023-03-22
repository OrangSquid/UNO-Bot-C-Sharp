using UNOLib.DrawStyle;

namespace UNOLib.StackStyles;

internal class NoStack : AbstractStackStyle
{
    public NoStack(IDrawStyle drawStyle) : base(drawStyle) { }

    public override bool ForcedDraw(ref GameState state, ICard card)
    {
        FinalDraw(ref state, HowManyCardsToDraw(card));
        return true;
    }
}