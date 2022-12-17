using System.ComponentModel;

namespace UNOLib
{
    public struct GameState
    {

        public ICard OnTable { get; internal set; }
        public IPlayer CurrentPlayer { get; internal set; }
        public bool JustReversedOrder { get; internal set; }
        public int CardsDrawn { get; internal set; }
        public int PlayersSkipped { get; internal set; }
        public bool JumpedIn { get; internal set; }
        public CardColors? ColorChanged { get => colorChanged; internal set => colorChanged = value; }
        internal bool ClockwiseOrder { get; set; }
        internal bool WaitingOnColorChange { get; set; }
        //internal int LeftToSkip { get; set; }
        //internal int CardsNextPlayerMustDraw { get; set; }

        private CardColors? colorChanged;

        internal GameState(ICard onTable, IPlayer currentPlayer) : this()
        {
            OnTable = onTable;
            CurrentPlayer = currentPlayer;
            ClockwiseOrder = true;
            WaitingOnColorChange = false;
            Refresh();
            //LeftToSkip = 0;
            //CardsNextPlayerMustDraw = 0;
        }

        internal void Refresh()
        {
            JustReversedOrder = false;
            CardsDrawn = 0;
            PlayersSkipped = 0;
            JumpedIn = false;
            colorChanged = null;
        }

        internal void ReverseOrder()
        {
            ClockwiseOrder = !ClockwiseOrder;
        }
    }
}
