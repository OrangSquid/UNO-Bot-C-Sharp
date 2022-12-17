namespace UNOLib
{
    public readonly struct Settings
    {
        public bool PlaySameSymbol { get; }
        public bool StackPlusTwo { get; }
        public bool StackWildPlusFour { get; }
        public bool MustPlay { get; }
        public bool JumpIn { get; }
        public bool DrawUntilPlayableCard { get; }
        public bool NumZeroPlayed { get; }
        public bool NumSevenPlayed { get; }
        public int NoUNOPenalty { get; }

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