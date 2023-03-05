using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOLib
{
    /// <summary>
    /// Draws cards until a playable one is found
    /// </summary>
    internal class DrawUntilFound : AbstractDrawStyle
    {
        public DrawUntilFound(List<ICard> fullDeck, int number_total_cards) : base(fullDeck, number_total_cards) { }

        public override bool GameDraw(GameState state)
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
}
