using UNOLib;

namespace UNOConsole
{
    public static class Program
    {
        private static GameSystem? uno;
        public static void Main(string[] args)
        {
            string? Command = "";
            while(Command is not null && !Command.Equals("exit"))
            {
                Command = Console.ReadLine();
                CommandInterpreter(Command);
            }
        }

        private static void CommandInterpreter(string? Command)
        {
            switch(Command)
            {
                case "new":
                    NewGame();
                    break;
                case "play":
                    PlayCard();
                    break;
                case "check":
                    CheckCards();
                    break;
            }
        }

        private static void NewGame()
        {
            Console.WriteLine("playSameSymbol");
            bool playSameSymbol = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("stackPlusTwo");
            bool stackPlusTwo = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("stackWildPlusFour");
            bool stackWildPlusFour = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("mustPlay");
            bool mustPlay = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("jumpIn");
            bool jumpIn = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("drawUntilPlayableCard");
            bool drawUntilPlayableCard = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("numZeroPlayed");
            bool numZeroPlayed = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("numSevenPlayed");
            bool numSevenPlayed = Convert.ToBoolean(Console.ReadLine());
            Console.WriteLine("noUNOPenalty");
            int noUNOPenalty = Convert.ToInt32(Console.ReadLine());

            Settings settings = new(playSameSymbol,
                                    stackPlusTwo,
                                    stackWildPlusFour,
                                    mustPlay,
                                    jumpIn,
                                    drawUntilPlayableCard,
                                    numZeroPlayed,
                                    numSevenPlayed,
                                    noUNOPenalty);

            Console.WriteLine("numPlayerd");
            uno = new(Convert.ToInt32(Console.ReadLine()), settings);

            Console.WriteLine("Game created");
        }

        private static void PlayCard()
        {

        }

        private static void CheckCards()
        {
            if(uno is null)
                return;
            int nPlayer = Convert.ToInt32(Console.ReadLine());
            IPlayer player = uno.GetPlayer(nPlayer);
            foreach(ICard card in player)
                Console.WriteLine(card.ToString());
        }
    }
}