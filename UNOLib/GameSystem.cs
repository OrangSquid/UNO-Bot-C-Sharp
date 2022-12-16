using System.Collections;
using System.Drawing;
using UNOLib.Exceptions;

namespace UNOLib
{
    public class GameSystem : IEnumerable<IPlayer>
    {
        private const int CARDS_PER_PLAYER = 7;
        private const int NUMBER_CARDS = 108;
        private const int NUMBER_OF_ZEROS = 1;
        private const int NUMBER_OF_EACH_COLOR_CARD = 2;
        private const int NUMBER_OF_EACH_WILD_CARD = 4;
        private static readonly Random rng = new();
        private static readonly Dictionary<string, ICard> _allCardsDict;
        private static readonly List<ICard> _allCards;
        private readonly Stack<ICard> _fullDeck;
        private readonly Settings _settings;
        private readonly List<IPlayer> _playersByOrder;
        private GameState _state;
        private ICard _onTable;
        private IPlayer _currentPlayer;

        public string OnTable => _onTable.ToString();

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

        public GameSystem(int nPlayers, Settings settings)
        {
            _fullDeck = new(_allCards.OrderBy(a => rng.Next()).ToList());
            _playersByOrder = new(nPlayers);
            for (int i = 0; i < nPlayers; i++)
            {
                IPlayer player = new Player(i);
                _playersByOrder.Add(player);
                for (int j = 0; j < CARDS_PER_PLAYER; j++)
                {
                    player.AddCard(_fullDeck.Pop());
                }
            }
            _currentPlayer = _playersByOrder.First();
            _onTable = _fullDeck.Pop();
            _settings = settings;
            _state = new GameState(_onTable, _currentPlayer);
        }

        public void CardPlay(string cardId)
        {
            if (!_allCardsDict.TryGetValue(cardId, out ICard? cardToBePlayed) || !_onTable.CanBePlayed(cardToBePlayed))
            {
                throw new CardCannotBePlayedException();
            }
            else
            {
                _currentPlayer.RemoveCard(cardId);
                _onTable = cardToBePlayed;
                CardAction(cardToBePlayed);
                SetNextPlayer();
            }
        }

        private void CardAction(ICard card)
        {
            if (card is WildCard wildCard)
            {
                switch (wildCard.Symbol)
                {
                    case WildCardSymbols.Simple: //Changes the color on the table
                        break;
                    case WildCardSymbols.PlusFour: //Changes the color on the table and
                                                   //the next player has to draw 4 cards
                        break;
                }
            }
            else if (card is ColorCard colorCard)
            {
                switch (colorCard.Symbol)
                {
                    // TODO
                    /* case ColorCardSymbols.Zero: //To be decided
                        break;
                    case ColorCardSymbols.Seven: //Ditto
                        break; */
                    case ColorCardSymbols.Skip: //Skips the next player
                        SetNextPlayer();
                        break;
                    case ColorCardSymbols.Reverse: //Reverses the order of players in the next round
                        _state.ReverseOrder();
                        if (_playersByOrder.Count == 2)
                            SetNextPlayer();
                        break;
                    case ColorCardSymbols.PlusTwo: //Next player has to draw 2 cards
                        // TODO make it stack
                        DrawAndSkip(2);
                        break;
                }
            }
        }

        public IPlayer GetPlayer(int id)
        {
            return _playersByOrder[id];
        }

        public void DrawCard()
        {
            _currentPlayer.AddCard(_fullDeck.Pop());
            SetNextPlayer();
        }

        public void DrawAndSkip(int cardsToDraw)
        {
            SetNextPlayer();
            for (int i = 0; i < cardsToDraw; i++)
                _currentPlayer.AddCard(_fullDeck.Pop());
        }

        private void SetNextPlayer()
        {
            if(_state.ClockwiseOrder)
                _currentPlayer = _playersByOrder[(_currentPlayer.Id + 1) % _playersByOrder.Count];
            else
                _currentPlayer = _playersByOrder[(_currentPlayer.Id + _playersByOrder.Count - 1) % _playersByOrder.Count];
        }

        // TOGO GET THIS TO GAME STATE
        public IPlayer getCurrentPlayer()
        {
            return _currentPlayer;
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
}