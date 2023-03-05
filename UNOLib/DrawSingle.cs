using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOLib
{
    /// <summary>
    /// Draw a single card
    /// </summary>
    internal class DrawSingle : AbstractDrawStyle
    {
        public DrawSingle(List<ICard> fullDeck, int number_total_cards) : base(fullDeck, number_total_cards) { }

        public override bool GameDraw(GameState state)
        {
            ICard card = Draw();
            state.CurrentPlayer.AddCard(card);
            state.CardsDrawn = 1;
            state.WhoDrewCards = state.CurrentPlayer;
            return state.OnTable.CanBePlayed(card);
        }
    }
}
