using System.ComponentModel;

namespace UNOLib
{
    public struct GameState
    {

        public ICard OnTable { get; internal set; }
        public LinkedList<ICard> CardsPlayed { get; internal set; }
        public IPlayer CurrentPlayer { get; internal set; }
        public IPlayer? PreviousPlayer { get; internal set; }
        public bool JustReversedOrder { get; internal set; }
        public List<IPlayer> PlayersSkiped { get; internal set; }
        public IPlayer? WhoDrewCards { get; internal set; }
        public int CardsDrawn { get; internal set; }
        public CardColors? ColorChanged { get => colorChanged; internal set => colorChanged = value; }
        public bool NewTurn { get; internal set; }
        internal bool ClockwiseOrder { get; set; }
        internal bool WaitingOnColorChange { get; set; }

        private CardColors? colorChanged;

        internal GameState(ICard onTable, IPlayer currentPlayer, int nPlayers)
        {
            OnTable = onTable;
            CurrentPlayer = currentPlayer;
            ClockwiseOrder = true;
            WaitingOnColorChange = false;
            PlayersSkiped = new(nPlayers);
            CardsPlayed = new();
            Refresh();
            NewTurn = true;
        }

        /// <summary>
        /// Refreshes the game state for a new command
        /// </summary>
        internal void Refresh()
        {
            JustReversedOrder = false;
            WhoDrewCards = null;
            CardsDrawn = 0;
            colorChanged = null;
            WaitingOnColorChange = false;
            PlayersSkiped.Clear();
            CardsPlayed.Clear();
            NewTurn = false;
        }

        internal void ReverseOrder()
        {
            ClockwiseOrder = !ClockwiseOrder;
            JustReversedOrder = true;
        }
    }
}
