using UNOLib;
using UNOLib.Exceptions;

namespace UNOConsole
{
    public static class Program
    {
        private static GameSystem? uno;
        public static void Main(string[] args)
        {
            string? Command = "";
            while (Command is not null && !Command.Equals("exit"))
            {
                Command = Console.ReadLine();
                CommandInterpreter(Command);
            }
        }

        private static void CommandInterpreter(string? Command)
        {
            switch (Command)
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
                case "draw":
                    DrawCard();
                    break;
                case "state":
                    CheckState();
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
            if (uno is null)
            {
                return;
            }
            string? cardId = Console.ReadLine();
            try
            {
                if (cardId != null)
                    uno.CardPlay(cardId);
            }
            catch (CardCannotBePlayedException)
            {
                Console.WriteLine("That Card cannot be played");
            }
            catch (PlayerDoesNotHaveCardException)
            {
                Console.WriteLine("Player does not have that card");
            }

            Console.WriteLine("----------");

        }

        private static void CheckCards()
        {
            if (uno is null)
            {
                return;
            }
            int nPlayer = Convert.ToInt32(Console.ReadLine());
            try
            {
                IPlayer player = uno.GetPlayer(nPlayer);
                foreach (ICard card in player)
                {
                    Console.WriteLine(card.ToString());
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Player does not exist");
            }
        }

        private static void DrawCard()
        {
            if (uno is null)
            {
                return;
            }

            uno.DrawCard();
        }

        private static void CheckState()
        {
            if (uno is null)
            {
                return;
            }
            foreach (IPlayer player in uno)
            {
                if(player == uno.State.CurrentPlayer)
                    Console.Write("-> ");
                else
                    Console.Write("   ");

                Console.WriteLine("P{0}: {1} cards", player.Id, player.NumCards);
            }
            Console.WriteLine(uno.State.OnTable);
        }
    }
}