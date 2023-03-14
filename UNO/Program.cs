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
                case "changeColor":
                    ChangeColor();
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
            StateInterpreter(true);
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
            StateInterpreter(false);
        }

        private static void ChangeColor()
        {
            if (uno is null)
            {
                return;
            }
            string? color = Console.ReadLine();
            try
            {
                uno.ChangeOnTableColor(color);
                Console.WriteLine("Color changed to: {0}", color);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid color");
            }
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
                    Console.WriteLine(card);
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
            StateInterpreter(false);
        }

        private static void CheckState()
        {
            if (uno is null)
            {
                return;
            }
            foreach (IPlayer player in uno)
            {
                if (player == uno.State.CurrentPlayer)
                    Console.Write("-> ");
                else
                    Console.Write("   ");

                Console.WriteLine("P{0}: {1} cards", player.Id, player.NumCards);
            }
            Console.WriteLine(uno.State.OnTable);
        }

        private static void StateInterpreter(bool newGame)
        {
            if (uno is null)
            {
                return;
            }

            Console.WriteLine("---------------");

            if (newGame)
            {
                Console.WriteLine("Game started");
                Console.WriteLine("Card on table: {0}", uno.State.OnTable);
            }
            else
            {
                // Played a card
                if (uno.State.CardsPlayed.Count != 0 && uno.State.PreviousPlayer != null)
                {
                    Console.WriteLine("Player {0} played:", uno.State.PreviousPlayer.Id);
                    foreach (ICard card in uno.State.CardsPlayed)
                    {
                        Console.WriteLine(card);
                    }

                }
                // Cards were drawn
                if (uno.State.WhoDrewCards != null)
                {
                    Console.WriteLine("Player {0} drew {1} cards", uno.State.WhoDrewCards.Id, uno.State.CardsDrawn);
                }
                // Players were skipped
                if (uno.State.PlayersSkiped.Count != 0)
                {
                    Console.WriteLine("These Players were skipped: ");
                    foreach (IPlayer player in uno.State.PlayersSkiped)
                    {
                        Console.WriteLine("Player {0}", player.Id);
                    }
                }
                // The order was reversed
                if (uno.State.JustReversedOrder)
                {
                    Console.WriteLine("The order has been reversed");
                }
                // Waiting for the color to change
                if (uno.State.WaitingOnColorChange)
                {
                    Console.WriteLine("Waiting for Player {0} to choose a color", uno.State.CurrentPlayer);
                }
            }
            if (uno.State.NewTurn)
            {
                Console.WriteLine("Your turn now: {0}", uno.State.CurrentPlayer.Id);
            }

            Console.WriteLine("---------------");
        }
    }
}