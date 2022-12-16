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
}