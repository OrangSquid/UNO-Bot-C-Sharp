namespace UNOLib;

public struct GameState
{
    public ICard OnTable { get; internal set; }
    public LinkedList<ICard> CardsPlayed { get; internal set; }
    public IPlayer CurrentPlayer { get; internal set; }
    public IPlayer? PreviousPlayer { get; internal set; }
    public bool JustReversedOrder { get; internal set; }
    public List<IPlayer> PlayersSkipped { get; internal set; }
    public IPlayer? WhoDrewCards { get; internal set; }
    // Note: CardsDrawn is set to a number when a wild plus 4 is played however nobody is set to have drawn those cards
    // since the system will be waiting for the color to change the wild
    public int CardsDrawn { get; internal set; }
    public CardColors? ColorChanged { get => colorChanged; internal set => colorChanged = value; }
    public bool NewTurn { get; internal set; }
    internal bool ClockwiseOrder { get; set; }
    public bool WaitingOnColorChange { get; internal set; }
    public bool GameFinished { get; internal set; }
    

    private CardColors? colorChanged;

    internal GameState(ICard onTable, IPlayer currentPlayer, int nPlayers)
    {
        OnTable = onTable;
        CurrentPlayer = currentPlayer;
        ClockwiseOrder = true;
        WaitingOnColorChange = false;
        PlayersSkipped = new(nPlayers);
        CardsPlayed = new();
        Refresh();
        NewTurn = true;
        GameFinished = false;
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
        PlayersSkipped.Clear();
        CardsPlayed.Clear();
        NewTurn = false;
    }

    /// <summary>
    /// Reverses the play order. Clockwise or Counter Clockwise
    /// </summary>
    internal void ReverseOrder()
    {
        ClockwiseOrder = !ClockwiseOrder;
        JustReversedOrder = true;
    }
}
