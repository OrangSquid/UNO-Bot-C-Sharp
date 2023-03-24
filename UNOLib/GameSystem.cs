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
    private readonly List<IPlayer> _playersByOrder;
    private readonly IDrawStyle _drawStyle;
    private readonly IStackStyle _stackStyle;
    private readonly bool _mustPlay;
    private readonly bool _jumpIn;
    private bool _clockwiseOrder;
    private GameState _state;
    
    public GameState State => _state;

    internal GameSystem(int nPlayers, Dictionary<string, ICard> allCardsDict, IDrawStyle drawStyle, bool mustPlay,
        IStackStyle stackStyle, bool jumpIn)
    {
        _playersByOrder = new(nPlayers);
        _allCardsDict = allCardsDict;
        _drawStyle = drawStyle;
        for (var i = 0; i < nPlayers; i++)
        {
            IPlayer player = new Player.Player(i);
            _playersByOrder.Add(player);
            for (var j = 0; j < CardsPerPlayer; j++)
            {
                player.AddCard(_drawStyle.Draw());
            }
        }

        var startingCard = _drawStyle.Draw();
        while(startingCard is WildCard or ColorCard { Symbol: ColorCardSymbols.Skip and ColorCardSymbols.PlusTwo and ColorCardSymbols.Reverse } )
        {
            _drawStyle.Push(startingCard);
            startingCard = _drawStyle.Draw();
        }
        
        _state = new GameState(startingCard, _playersByOrder.First(), _playersByOrder.Count);
        _mustPlay = mustPlay;
        _stackStyle = stackStyle;
        _jumpIn = jumpIn;
        _clockwiseOrder = true;
    }

    public void CardPlay(int playerId, string cardId)
    {
        if (_state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
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
            if (!_jumpIn || _jumpIn && ReferenceEquals(_state.OnTable, cardToBePlayed))
            {
                throw new NotPlayersTurnException();
            }
            _state.CurrentPlayer = GetPlayer(playerId);
            _state.JumpedIn = true;
        }
        _state.CurrentPlayer.RemoveCard(cardId);
        _state.Refresh();
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
        return _playersByOrder[id];
    }

    public void DrawCard(int playerId)
    {
        if (_state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
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
        if (_state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
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

    public void ChangeOnTableColor(int playerId, string color)
    {
        if (_state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
        if (!_state.WaitingOnColorChange || _state.OnTable is WildCard)
        {
            throw new CannotChangeColorException();
        }
        if (playerId != _state.CurrentPlayer.Id)
        {
            throw new NotPlayersTurnException();
        }
        var wildCard = _state.OnTable as WildCard;
        wildCard.Color = Enum.Parse<CardColors>(color);
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
        if (card is WildCard)
        {
            _state.WaitingOnColorChange = true;
        }
        else if (card is ColorCard colorCard)
        {
            switch (colorCard.Symbol)
            {
                case ColorCardSymbols.Skip: //Skips the next player
                    SkipPlayer();
                    break;
                case ColorCardSymbols.Reverse: //Reverses the order of players in the next round
                    ReverseOrder();
                    if (_playersByOrder.Count == 2)
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
            : _state.CurrentPlayer.Id + _playersByOrder.Count - 1;
        _state.CurrentPlayer = _playersByOrder[playerId % _playersByOrder.Count];
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
        return ((IEnumerable<IPlayer>)_playersByOrder).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_playersByOrder).GetEnumerator();
    }
}