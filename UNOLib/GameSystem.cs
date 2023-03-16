﻿using System.Collections;
using UNOLib.Exceptions;

namespace UNOLib;

public class GameSystem : IEnumerable<IPlayer>
{
    private const int CARDS_PER_PLAYER = 7;
    private const int NUMBER_CARDS = 108;
    private const int NUMBER_OF_ZEROS = 1;
    private const int NUMBER_OF_EACH_COLOR_CARD = 2;
    private const int NUMBER_OF_EACH_WILD_CARD = 4;
    private static readonly Dictionary<string, ICard> _allCardsDict;
    private static readonly List<ICard> _allCards;

    private readonly Settings _settings;
    private readonly List<IPlayer> _playersByOrder;
    private readonly IDrawStyle _drawStyle;
    private GameState _state;

    public GameState State { get => _state; }

    static GameSystem()
    {
        _allCardsDict = new(NUMBER_CARDS);
        _allCards = new(NUMBER_CARDS);
        foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            foreach (ColorCardSymbols symbol in Enum.GetValuesAsUnderlyingType<ColorCardSymbols>())
            {
                ICard card = new ColorCard(color, symbol);
                _allCardsDict.Add(card.ToString(), card);
                int i = symbol == ColorCardSymbols.Zero ? NUMBER_OF_ZEROS : NUMBER_OF_EACH_COLOR_CARD;
                for (; i > 0; i--)
                {
                    _allCards.Add(card);
                }
            }
        }
        foreach (WildCardSymbols symbol in Enum.GetValuesAsUnderlyingType<WildCardSymbols>())
        {
            // CardColors.RED is simply used as a placeholder
            ICard card = new WildCard(CardColors.Red, symbol);
            _allCardsDict.Add(card.ToString(), card);
            for (int i = 0; i < NUMBER_OF_EACH_WILD_CARD; i++)
            {
                _allCards.Add(card);
            }
        }
    }

    /// <summary>
    /// Game System constructor. Received the number of players and the chosen settings.
    /// </summary>
    /// <param name="nPlayers">Number of Players in the game</param>
    /// <param name="settings">Settings object containing the chosen settings</param>
    public GameSystem(int nPlayers, Settings settings)
    {
        // Create DrawStyle
        if (settings.DrawUntilPlayableCard)
        {
            _drawStyle = new DrawUntilFound(_allCards, NUMBER_CARDS);
        }
        else
        {
            _drawStyle = new DrawSingle(_allCards, NUMBER_CARDS);
        }
        _playersByOrder = new(nPlayers);
        for (int i = 0; i < nPlayers; i++)
        {
            IPlayer player = new Player(i);
            _playersByOrder.Add(player);
            for (int j = 0; j < CARDS_PER_PLAYER; j++)
            {
                player.AddCard(_drawStyle.Draw());
            }
        }
        _settings = settings;
        _state = new GameState(_drawStyle.Draw(), _playersByOrder.First(), _playersByOrder.Count);
    }

    /// <summary>
    /// Plays the card and checks if the players has finished their deck. 
    /// CardAction is called afterwards to do the action specified for that card.
    /// </summary>
    /// <param name="cardId">Id of the card to be played</param>
    /// <exception cref="GameIsFinishedException">Game is already over. Cannot do more actions.</exception>
    /// <exception cref="CardCannotBePlayedException">Card does not meet the requirements to be played or does not exist at all.</exception>
    public void CardPlay(int playerId, string cardId)
    {
        if (_state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
        // Check if card is present in the dictionary for all cards and if card can be played on top of current one
        if (!_allCardsDict.TryGetValue(cardId, out ICard? cardToBePlayed) || !_state.OnTable.CanBePlayed(cardToBePlayed))
        {
            throw new CardCannotBePlayedException();
        }
        if (playerId != _state.CurrentPlayer.Id)
        {
            throw new NotPlayersTurnException();
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

    /// <summary>
    /// Gets the player with the specified Id
    /// </summary>
    /// <param name="id">Id of the Player</param>
    /// <returns></returns>
    public IPlayer GetPlayer(int id)
    {
        return _playersByOrder[id];
    }

    /// <summary>
    /// Should be caled when the current player wants or needs to draw cards
    /// </summary>
    /// <exception cref="GameIsFinishedException">Game is already over. Cannot do more actions.</exception>
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
        _state.Refresh();
        _state.PreviousPlayer = _state.CurrentPlayer;
        if (!_drawStyle.GameDraw(ref _state))
            SetNextPlayer();
    }

    /// <summary>
    /// Changes the color when a Wild Card is played to the specified by the current Player.
    /// If a Four Plus Wild Card is played it also makes the next player draw 4 cards and skips it.
    /// </summary>
    /// <param name="color">color that was chosen</param>
    /// <exception cref="GameIsFinishedException">Game is already over. Cannot do more actions.</exception>
    /// <exception cref="CannotChangeColorException">Cannot change the color because there was no Wild Card played.</exception>
    /// <exception cref="ArgumentException">color is invalid.</exception>
    public void ChangeOnTableColor(int playerId, string color)
    {
        if (_state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
        else if (!_state.WaitingOnColorChange)
        {
            throw new CannotChangeColorException();
        }
        if (playerId != _state.CurrentPlayer.Id)
        {
            throw new NotPlayersTurnException();
        }
        else if (_state.OnTable is WildCard wildCard)
        {
            wildCard.Color = Enum.Parse<CardColors>(color);
            _state.ColorChanged = wildCard.Color;
            _state.WaitingOnColorChange = false;
            if (_state.CardsDrawn != 0)
            {
                DrawAndSkip(_state.CardsDrawn);
            }
            SetNextPlayer();
        }
    }

    /// <summary>
    /// Selects the appropriate method for the card played. ColorCards sets the next turn, WildCards wait for the color.
    /// </summary>
    /// <param name="card">Card that was played</param>
    private void CardAction(ICard card)
    {
        if (card is WildCard wildCard)
        {
            switch (wildCard.Symbol)
            {
                case WildCardSymbols.Simple: //Changes the color on the table
                    _state.WaitingOnColorChange = true;
                    break;
                case WildCardSymbols.PlusFour: //Changes the color on the table and the next player has to draw 4 cards
                    _state.WaitingOnColorChange = true;
                    _state.CardsDrawn = 4;
                    break;
                default:
                    break;
            }
        }
        else if (card is ColorCard colorCard)
        {
            switch (colorCard.Symbol)
            {
                case ColorCardSymbols.Skip: //Skips the next player
                    SkipPlayer();
                    break;
                case ColorCardSymbols.Reverse: //Reverses the order of players in the next round
                    _state.ReverseOrder();
                    if (_playersByOrder.Count == 2)
                        SelectNextPlayer();
                    break;
                case ColorCardSymbols.PlusTwo: //Next player has to draw 2 cards
                    // TODO make it stack
                    DrawAndSkip(2);
                    break;
                default:
                    break;
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
        if (_state.ClockwiseOrder)
            _state.CurrentPlayer = _playersByOrder[(_state.CurrentPlayer.Id + 1) % _playersByOrder.Count];
        else
            _state.CurrentPlayer = _playersByOrder[(_state.CurrentPlayer.Id + _playersByOrder.Count - 1) % _playersByOrder.Count];
    }

    /// <summary>
    /// Should be called when the next player is forced to draw cards
    /// </summary>
    /// <param name="cardsToDraw">The number of cards that must be drawn</param>
    private void DrawAndSkip(int cardsToDraw)
    {
        SkipPlayer();
        for (int i = 0; i < cardsToDraw; i++)
            _state.CurrentPlayer.AddCard(_drawStyle.Draw());
        _state.CardsDrawn = cardsToDraw;
        _state.WhoDrewCards = _state.CurrentPlayer;
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
    /// Skips the next player when a Skip Card
    /// </summary>
    private void SkipPlayer()
    {
        SelectNextPlayer();
        _state.PlayersSkiped.Add(_state.CurrentPlayer);
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