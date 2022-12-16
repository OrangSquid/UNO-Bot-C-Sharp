using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOLib
{
    public struct GameState
    {
        public ICard OnTable { get; set; }
        public IPlayer CurrentPlayer { get; set; }
        internal bool ClockwiseOrder { get; set; }
        internal bool WaitingOnColorChange { get; set; }
        //internal int LeftToSkip { get; set; }
        //internal int CardsNextPlayerMustDraw { get; set; }

        internal GameState(ICard onTable, IPlayer currentPlayer) : this()
        {
            OnTable = onTable;
            CurrentPlayer = currentPlayer;
            ClockwiseOrder = true;
            WaitingOnColorChange = false;
            //LeftToSkip = 0;
            //CardsNextPlayerMustDraw = 0;
        }

        internal void ReverseOrder()
        {
            ClockwiseOrder = !ClockwiseOrder;
        }
    }
}
