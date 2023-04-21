using UNOLib.Cards;
using UNOLib.Player;

namespace UNOLib;

public record struct GameState
{
    public ICard OnTable { get; internal set; }
    public LinkedList<ICard> CardsPlayed { get; }
    public IPlayer CurrentPlayer { get; internal set; }
    public IPlayer? PreviousPlayer { get; internal set; }
    public bool JustReversedOrder { get; internal set; }
    public List<IPlayer> PlayersSkipped { get; }
    public IPlayer? WhoDrewCards { get; internal set; }
    // Note: CardsDrawn is set to a number when a wild plus 4 is played however nobody is set to have drawn those cards
    // since the system will be waiting for the color to change the wild
    public int CardsDrawn { get; internal set; }
    public CardColors? ColorChanged { get => _colorChanged; internal set => _colorChanged = value; }
    public bool NewTurn { get; internal set; }
    public bool WaitingOnColorChange { get; internal set; }
    public bool GameFinished { get; internal set; }
    internal bool CanSkip { get; set; }
    internal bool HasDrawnCards { get; set; }
    public bool HasSkipped { get; internal set; }
    public bool StackPlusTwo { get; internal set; }
    public bool StackPlusFour { get; internal set; }
    public bool JumpedIn { get; internal set; }
    public IPlayer? WaitingOnUno { get; internal set; }
    public IPlayer? YelledUno { get; internal set; }


    private CardColors? _colorChanged;

    internal GameState(ICard onTable, IPlayer currentPlayer, int nPlayers)
    {
        OnTable = onTable;
        CurrentPlayer = currentPlayer;
        WaitingOnColorChange = false;
        PlayersSkipped = new List<IPlayer>(nPlayers);
        CardsPlayed = new LinkedList<ICard>();
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
        if (WhoDrewCards != null)
        {
            CardsDrawn = 0;
        }
        WhoDrewCards = null;
        _colorChanged = null;
        WaitingOnColorChange = false;
        PlayersSkipped.Clear();
        CardsPlayed.Clear();
        NewTurn = false;
        CanSkip = false;
        HasDrawnCards = false;
        HasSkipped = false;
        StackPlusFour = false;
        StackPlusTwo = false;
        JumpedIn = false;
        WaitingOnUno = null;
        YelledUno = null;
    }
}