using UNOLib.Exceptions;

namespace UNOLib
{
    public readonly struct Settings
    {
        public bool PlaySameSymbol { get; init; }
        public bool StackPlusTwo { get; init; }
        public bool StackWildPlusFour { get; init; }
        public bool MustPlay { get; init; }
        public bool JumpIn { get; init; }
        public bool DrawUntilPlayableCard { get; init; }
        public bool NumZeroPlayed { get; init; }
        public bool NumSevenPlayed { get; init; }
        public int NoUNOPenalty { get; init; }

        public Settings(bool playSameSymbol,
                        bool stackPlusTwo,
                        bool stackWildPlusFour,
                        bool mustPlay,
                        bool jumpIn,
                        bool drawUntilPlayableCard,
                        bool numZeroPlayed,
                        bool numSevenPlayed,
                        int noUNOPenalty)
        {
            PlaySameSymbol = playSameSymbol;
            StackPlusTwo = stackPlusTwo;
            StackWildPlusFour = stackWildPlusFour;
            MustPlay = mustPlay;
            JumpIn = jumpIn;
            DrawUntilPlayableCard = drawUntilPlayableCard;
            NumZeroPlayed = numZeroPlayed;
            NumSevenPlayed = numSevenPlayed;
            NoUNOPenalty = noUNOPenalty;
        }
    }

    public class GameSystem
    {
        private const int CARDS_PER_PLAYER = 7;
        private const int NUMBER_CARDS = 108;
        private static readonly Random rng = new();
        private static readonly Dictionary<string, ICard> _allCardsDict;
        private static readonly List<ICard> _allCards;
        private readonly Stack<ICard> _fullDeck;
        private readonly Settings _settings;
        private readonly List<IPlayer> _playersByOrder;
        private ICard _onTable;
        private IPlayer _currentPlayer;

        static GameSystem()
        {
            _allCardsDict = new(NUMBER_CARDS);
            _allCards = new(NUMBER_CARDS);
            foreach (CardColors color in Enum.GetValuesAsUnderlyingType<CardColors>())
            {
                foreach (ColorCardSymbols symbol in Enum.GetValuesAsUnderlyingType<ColorCardSymbols>())
                {
                    // TODO find better implementation for card counting
                    ICard card = new ColorCard(color, symbol);
                    _allCardsDict.Add(card.ToString(), card);
                    int i;
                    if (symbol == ColorCardSymbols.Zero) i = 1;
                    else i = 2;
                    for (; i > 0; i--)
                        _allCards.Add(card);
                }
            }
            foreach (WildCardSymbols symbol in Enum.GetValuesAsUnderlyingType<WildCardSymbols>())
            {
                // CardColors.RED is simply used as a placeholder
                // TODO find better implementation for card counting
                ICard card = new WildCard(CardColors.Red, symbol);
                _allCardsDict.Add(card.ToString(), card);
                for (int i = 0; i < 4; i++)
                    _allCards.Add(card);
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
            _currentPlayer = _playersByOrder.First<IPlayer>();
            _onTable = _fullDeck.Pop();
            _settings = settings;
        }

        public void CardPlay(string cardId)
        {
            if (!_allCardsDict.TryGetValue(cardId, out ICard? cardToBePlayed))
                throw new CardCannotBePlayedException();
            else if (!_onTable.CanBePlayed(cardToBePlayed))
                throw new CardCannotBePlayedException();
            else
            {
                _onTable = cardToBePlayed;
                _currentPlayer.RemoveCard(cardId);
                _currentPlayer = _playersByOrder[(_currentPlayer.Id + 1) % _playersByOrder.Count];
            }
        }

        public IPlayer GetPlayer(int id)
        {
            return _playersByOrder[id];
        }
    }
}