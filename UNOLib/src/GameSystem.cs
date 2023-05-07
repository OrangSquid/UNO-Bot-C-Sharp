using System.Collections;
using UNOLib.Cards;
using UNOLib.DrawStyle;
using UNOLib.Exceptions;
using UNOLib.Player;
using UNOLib.StackStyles;

namespace UNOLib;

public class GameSystem : IGameSystem
{
    private const int CardsPerPlayer = 7;

    private readonly Dictionary<string, ICard> _allCardsDict;
    protected readonly List<IPlayer> PlayersByOrder;
    private readonly IDrawStyle _drawStyle;
    private readonly IStackStyle _stackStyle;
    private readonly bool _mustPlay;
    private readonly bool _jumpIn;
    private readonly int _unoPenalty;
    private bool _clockwiseOrder;
    private GameState _state;

    public GameState State => _state;

    protected internal GameSystem(List<IPlayer> playersByOrder, Dictionary<string, ICard> allCardsDict, IDrawStyle drawStyle, bool mustPlay,
        IStackStyle stackStyle, bool jumpIn, int unoPenalty)
    {
        PlayersByOrder = playersByOrder;
        _allCardsDict = allCardsDict;
        _drawStyle = drawStyle;
        foreach (var player in playersByOrder)
        {
            for (var j = 0; j < CardsPerPlayer; j++)
            {
                player.AddCard(_drawStyle.Draw());
            }
        }

        var startingCard = _drawStyle.Draw();
        while (startingCard is WildCard or ColorCard
               {
                   Symbol: ColorCardSymbols.Skip or ColorCardSymbols.PlusTwo or ColorCardSymbols.Reverse
               })
        {
            _drawStyle.Push(startingCard);
            startingCard = _drawStyle.Draw();
        }

        _state = new GameState(startingCard, PlayersByOrder.First(), PlayersByOrder.Count);
        _mustPlay = mustPlay;
        _stackStyle = stackStyle;
        _jumpIn = jumpIn;
        _unoPenalty = unoPenalty;
        _clockwiseOrder = true;
    }

    public void CardPlay(int playerId, string cardId)
    {
        GuardClauses.GuardAgainstFinishedGame(_state);

        // Check if card is present in the dictionary for all cards and if card can be played on top of current one
        if (!_allCardsDict.TryGetValue(cardId, out var cardToBePlayed))
        {
            throw new CardCannotBePlayedException();
        }
        if (_state.StackPlusTwo && cardToBePlayed is ColorCard colorCard && colorCard.Symbol != ColorCardSymbols.PlusTwo)
        {
            throw new CardCannotBePlayedException();
        }
        if (!_state.OnTable.CanBePlayed(cardToBePlayed))
        {
            throw new CardCannotBePlayedException();
        }
        if (playerId != _state.CurrentPlayer.Id)
        {
            if (!_jumpIn || _jumpIn && !ReferenceEquals(_state.OnTable, cardToBePlayed))
            {
                throw new NotPlayersTurnException();
            }
            _state.CurrentPlayer = GetPlayer(playerId);
            _state.JumpedIn = true;
        }
        _state.Refresh();
        // Set waiting on UNO once a player has only 1 card in hand
        if (_state.CurrentPlayer.RemoveCard(cardId))
        {
            _state.WaitingOnUno = _state.CurrentPlayer;
        }
        _state.PreviousPlayer = _state.CurrentPlayer;
        _drawStyle.Push(_state.OnTable);
        _state.OnTable = cardToBePlayed;
        _state.CardsPlayed.AddLast(cardToBePlayed);
        if (_state.CurrentPlayer.NumCards == 0)
        {
            _state.GameFinished = true;
        }
        else
        {
            CardAction(cardToBePlayed);
        }
    }

    public IPlayer GetPlayer(int id)
    {
        return PlayersByOrder[id];
    }

    public void DrawCard(int playerId)
    {
        GuardClauses.GuardAgainstFinishedGame(_state);

        if (playerId != _state.CurrentPlayer.Id)
        {
            throw new NotPlayersTurnException();
        }
        if (_state.HasDrawnCards)
        {
            throw new PlayerCannotDrawException();
        }
        _state.Refresh();
        _state.PreviousPlayer = _state.CurrentPlayer;
        if (_state.CardsDrawn != 0)
        {
            _stackStyle.ForcedDraw(ref _state);
            SetNextPlayer();
        }
        else if (!_drawStyle.GameDraw(ref _state))
        {
            SetNextPlayer();
        }
        else
        {
            _state.HasDrawnCards = true;
            if (!_mustPlay)
            {
                _state.CanSkip = true;
            }
        }
    }

    public void Skip(int playerId)
    {
        GuardClauses.GuardAgainstFinishedGame(_state);

        if (playerId != _state.CurrentPlayer.Id)
        {
            throw new NotPlayersTurnException();
        }
        if (!_state.CanSkip)
        {
            throw new PlayerCannotSkipException();
        }
        _state.Refresh();
        _state.PreviousPlayer = _state.CurrentPlayer;
        _state.HasSkipped = true;
        SetNextPlayer();
    }

    public void Uno(int playerId)
    {
        if (_state.WaitingOnUno == null)
        {
            throw new NobodyHasOnlyOneCardException();
        }
        var waitingOnUno = _state.WaitingOnUno;
        _state.Refresh();
        _state.YelledUno = GetPlayer(playerId);
        if (!ReferenceEquals(waitingOnUno, _state.YelledUno))
        {
            for (var j = 0; j < _unoPenalty; j++)
            {
                waitingOnUno.AddCard(_drawStyle.Draw());
            }
            _state.WhoDrewCards = waitingOnUno;
            _state.CardsDrawn = _unoPenalty;
        }
        _state.NewTurn = true;
    }

    public void ChangeOnTableColor(int playerId, string color)
    {
        GuardClauses.GuardAgainstFinishedGame(_state);

        if (!_state.WaitingOnColorChange || _state.OnTable is not WildCard wildCard)
        {
            throw new CannotChangeColorException();
        }
        if (playerId != _state.CurrentPlayer.Id)
        {
            throw new NotPlayersTurnException();
        }
        _state.OnTable = wildCard.ChangeColor(color); 
        _drawStyle.Push(wildCard);
        _state.ColorChanged = wildCard.Color;
        _state.WaitingOnColorChange = false;
        SetNextPlayer();
        if (wildCard.Symbol == WildCardSymbols.PlusFour && _stackStyle.ForcedDraw(ref _state, wildCard))
        {
            _state.PlayersSkipped.Add(_state.CurrentPlayer);
            SetNextPlayer();
        }
    }

    /// <summary>
    /// Selects the appropriate method for the card played. ColorCards sets the next turn, WildCards wait for the color.
    /// </summary>
    /// <param name="card">Card that was played</param>
    private void CardAction(ICard card)
    {
        switch (card)
        {
            case WildCard:
                _state.WaitingOnColorChange = true;
                break;
            case ColorCard colorCard:
                switch (colorCard.Symbol)
                {
                    case ColorCardSymbols.Skip: //Skips the next player
                        SkipPlayer();
                        break;
                    case ColorCardSymbols.Reverse: //Reverses the order of players in the next round
                        ReverseOrder();
                        if (PlayersByOrder.Count == 2)
                            SelectNextPlayer();
                        break;
                    case ColorCardSymbols.PlusTwo: //Next player has to draw 2 cards
                        // Sets up the next player for the forced draw
                        SetNextPlayer();
                        if (_stackStyle.ForcedDraw(ref _state, colorCard))
                        {
                            _state.PlayersSkipped.Add(_state.CurrentPlayer);
                            SetNextPlayer();
                        }
                        // DO NOT SET UP NEXT PLAYER
                        return;
                }
                SetNextPlayer();
                break;
        }
    }

    /// <summary>
    /// Sets new current player. Does not make it ready for a new turn. Should only be used for
    /// as an auxilary method for skipping and forcibly drawing cards.
    /// </summary>
    private void SelectNextPlayer()
    {
        int playerId = _clockwiseOrder
            ? _state.CurrentPlayer.Id + 1
            : _state.CurrentPlayer.Id + PlayersByOrder.Count - 1;
        _state.CurrentPlayer = PlayersByOrder[playerId % PlayersByOrder.Count];
    }

    /// <summary>
    /// Sets new current player and prepares it for a new turn.
    /// </summary>
    private void SetNextPlayer()
    {
        SelectNextPlayer();
        _state.NewTurn = true;
    }

    /// <summary>
    /// Skips the next player when a Skip Card is played
    /// </summary>
    private void SkipPlayer()
    {
        SelectNextPlayer();
        _state.PlayersSkipped.Add(_state.CurrentPlayer);
    }

    /// <summary>
    /// Reverses the play order. Clockwise or Counter Clockwise
    /// </summary>
    private void ReverseOrder()
    {
        _clockwiseOrder = !_clockwiseOrder;
        _state.JustReversedOrder = true;
    }

    public IEnumerator<IPlayer> GetEnumerator()
    {
        return ((IEnumerable<IPlayer>)PlayersByOrder).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)PlayersByOrder).GetEnumerator();
    }
}