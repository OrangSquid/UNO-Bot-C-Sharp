using System.ComponentModel;

namespace UNOLib
{
    public struct GameState
    {

        public ICard OnTable { get; internal set; }
        public LinkedList<ICard> CardsPlayed { get; internal set; }
        public IPlayer CurrentPlayer { get; internal set; }
        public bool JustReversedOrder { get; internal set; }
        public List<IPlayer> PlayersSkiped { get; internal set; }
        public IPlayer? WhoDrewCards { get; internal set; }
        public int CardsDrawn { get; internal set; }
        public CardColors? ColorChanged { get => colorChanged; internal set => colorChanged = value; }
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
        }

        internal void Refresh()
        {
            JustReversedOrder = false;
            WhoDrewCards = null;
            CardsDrawn = 0;
            colorChanged = null;
            WaitingOnColorChange = false;
            PlayersSkiped.Clear();
            CardsPlayed.Clear();
        }

        internal void ReverseOrder()
        {
            ClockwiseOrder = !ClockwiseOrder;
            JustReversedOrder = true;
        }
    }
}
