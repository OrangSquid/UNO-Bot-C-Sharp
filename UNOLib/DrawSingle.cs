﻿namespace UNOLib;

/// <summary>
/// Draw a single card
/// </summary>
internal class DrawSingle : AbstractDrawStyle
{
    public DrawSingle(List<ICard> fullDeck, int number_total_cards) : base(fullDeck, number_total_cards) { }

    public override bool GameDraw(ref GameState state)
    {
        ICard card = Draw();
        state.CurrentPlayer.AddCard(card);
        state.CardsDrawn = 1;
        state.WhoDrewCards = state.CurrentPlayer;
        return state.OnTable.CanBePlayed(card);
    }
}
