namespace UNOLib.DrawStyle;

/// <summary>
/// Draws cards until a playable one is found
/// </summary>
internal class DrawUntilFound : AbstractDrawStyle
{
    public DrawUntilFound(List<ICard> fullDeck, int numberTotalCards) : base(fullDeck, numberTotalCards) { }

    public override bool GameDraw(ref GameState state)
    {
        ICard card;
        state.WhoDrewCards = state.CurrentPlayer;
        do
        {
            card = Draw();
            state.CurrentPlayer.AddCard(card);
            state.CardsDrawn++;
        } while (!state.OnTable.CanBePlayed(card));
        return true;
    }
}