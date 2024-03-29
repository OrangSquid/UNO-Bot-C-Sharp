﻿using UNOLib;
using UNOLib.Exceptions;

namespace UNOConsole;

public static class Program
{
    private static IGameSystem? _uno;
    public static void Main(string[] args)
    {
        var command = "";
        while (command is not null && !command.Equals("exit"))
        {
            command = Console.ReadLine();
            CommandInterpreter(command);
        }
    }

    private static void CommandInterpreter(string? command)
    {
        switch (command)
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
            case "skip":
                Skip();
                break;
            case "uno":
                Uno();
                break;
            default:
                Console.WriteLine("No such command");
                break;
        }
    }

    private static void NewGame()
    {
        //Console.WriteLine("playSameSymbol");
        //bool playSameSymbol = Convert.ToBoolean(Console.ReadLine());
        Console.WriteLine("stackPlusTwo");
        var stackPlusTwo = Convert.ToBoolean(Console.ReadLine());
        //Console.WriteLine("stackWildPlusFour");
        //bool stackWildPlusFour = Convert.ToBoolean(Console.ReadLine());
        Console.WriteLine("mustPlay");
        var mustPlay = Convert.ToBoolean(Console.ReadLine());
        Console.WriteLine("jumpIn");
        var jumpIn = Convert.ToBoolean(Console.ReadLine());
        Console.WriteLine("drawUntilPlayableCard");
        var drawUntilPlayableCard = Convert.ToBoolean(Console.ReadLine());
        //Console.WriteLine("numZeroPlayed");
        //bool numZeroPlayed = Convert.ToBoolean(Console.ReadLine());
        //Console.WriteLine("numSevenPlayed");
        //bool numSevenPlayed = Convert.ToBoolean(Console.ReadLine());
        Console.WriteLine("UNOPenalty");
        var unoPenalty = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("numPlayers");
        var nPlayers = Convert.ToInt32(Console.ReadLine());
        var gsf = new GameSystemBuilder()
            .CreatePlayers(nPlayers)
            .WithDrawUntilPlayable(drawUntilPlayableCard)
            .WithStackPlusTwo(stackPlusTwo)
            .WithMustPlay(mustPlay)
            .WithJumpIn(jumpIn)
            .WithUnoPenalty(unoPenalty)
            .Build();

        Console.WriteLine("Game created");
        StateInterpreter(true);
    }

    private static void PlayCard()
    {
        if (_uno is null)
        {
            return;
        }
        string? cardId = Console.ReadLine();
        var playerId = Convert.ToInt32(Console.ReadLine());
        try
        {
            if (cardId != null)
                _uno.CardPlay(playerId, cardId);
            StateInterpreter(false);
        }
        catch (CardCannotBePlayedException)
        {
            Console.WriteLine("That Card cannot be played");
        }
        catch (PlayerDoesNotHaveCardException)
        {
            Console.WriteLine("Player does not have that card");
        }
        catch (GameIsFinishedException)
        {
            Console.WriteLine("Games has finished");
        }
        catch (NotPlayersTurnException)
        {
            Console.WriteLine("Not player's turn");
        }
    }

    private static void ChangeColor()
    {
        if (_uno is null)
        {
            return;
        }
        string? color = Console.ReadLine();
        var playerId = Convert.ToInt32(Console.ReadLine());
        try
        {
            if (color != null)
                _uno.ChangeOnTableColor(playerId, color);
            StateInterpreter(false);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Invalid color");
        }
    }

    private static void CheckCards()
    {
        if (_uno is null)
        {
            return;
        }
        var nPlayer = Convert.ToInt32(Console.ReadLine());
        try
        {
            var player = _uno.GetPlayer(nPlayer);
            foreach (var card in player)
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
        if (_uno is null)
        {
            return;
        }

        try
        {
            var playerId = Convert.ToInt32(Console.ReadLine());
            _uno.DrawCard(playerId);
            StateInterpreter(false);
        }
        catch (GameIsFinishedException)
        {
            Console.WriteLine("Games has finished");
        }
        catch (NotPlayersTurnException)
        {
            Console.WriteLine("Not player's turn");
        }
        catch (PlayerCannotDrawException)
        {
            Console.WriteLine("You cannot draw");
        }
    }

    private static void CheckState()
    {
        if (_uno is null)
        {
            return;
        }
        foreach (var player in _uno)
        {
            Console.Write(player == _uno.State.CurrentPlayer ? "-> " : "   ");

            Console.WriteLine("P{0}: {1} cards", player.Id, player.NumCards);
        }
        Console.WriteLine(_uno.State.OnTable);
    }

    private static void Skip()
    {
        if (_uno is null)
        {
            return;
        }
        try
        {
            var playerId = Convert.ToInt32(Console.ReadLine());
            _uno.Skip(playerId);
            StateInterpreter(false);
        }
        catch (GameIsFinishedException)
        {
            Console.WriteLine("Games has finished");
        }
        catch (NotPlayersTurnException)
        {
            Console.WriteLine("Not player's turn");
        }
        catch (PlayerCannotSkipException)
        {
            Console.WriteLine("You cannot skip");
        }
    }

    private static void Uno()
    {
        if (_uno is null)
        {
            return;
        }

        try
        {
            var playerId = Convert.ToInt32(Console.ReadLine());
            _uno.Uno(playerId);
            StateInterpreter(false);
        }
        catch (GameIsFinishedException)
        {
            Console.WriteLine("Games has finished");
        }
        catch (NobodyHasOnlyOneCardException)
        {
            Console.WriteLine("Nobody has only one card");
        }
    }

    private static void StateInterpreter(bool newGame)
    {
        if (_uno is null)
        {
            return;
        }

        Console.WriteLine("---------------");

        if (newGame)
        {
            Console.WriteLine("Game started");
            Console.WriteLine("Card on table: {0}", _uno.State.OnTable);
        }
        else
        {
            // Played a card
            if (_uno.State.CardsPlayed.Count != 0 && _uno.State.PreviousPlayer != null)
            {
                Console.WriteLine("Player {0} played:", _uno.State.PreviousPlayer.Id);
                foreach (var card in _uno.State.CardsPlayed)
                {
                    Console.WriteLine(card);
                }

            }
            if (_uno.State.YelledUno != null)
            {
                Console.WriteLine("Player {0} yelled Uno", _uno.State.YelledUno);
            }
            // Cards were drawn
            if (_uno.State.WhoDrewCards != null)
            {
                Console.WriteLine("Player {0} drew {1} cards", _uno.State.WhoDrewCards.Id, _uno.State.CardsDrawn);
            }
            // Players were skipped
            if (_uno.State.PlayersSkipped.Count != 0)
            {
                Console.WriteLine("These Players were skipped: ");
                foreach (var player in _uno.State.PlayersSkipped)
                {
                    Console.WriteLine("Player {0}", player.Id);
                }
            }
            // The order was reversed
            if (_uno.State.JustReversedOrder)
            {
                Console.WriteLine("The order has been reversed");
            }
            // Waiting for the color to change
            if (_uno.State.WaitingOnColorChange)
            {
                Console.WriteLine("Waiting for Player {0} to choose a color", _uno.State.CurrentPlayer.Id);
            }
            if (_uno.State.ColorChanged != null)
            {
                Console.WriteLine("Color changed to: {0}", _uno.State.ColorChanged);
            }
            if (_uno.State.HasSkipped)
            {
                Console.WriteLine("Player {0} has skipped their turn", _uno.State.PreviousPlayer);
            }
            if (_uno.State.GameFinished)
            {
                Console.WriteLine("Game has ended");
            }
        }
        if (_uno.State.NewTurn)
        {
            Console.WriteLine("Your turn now: {0}", _uno.State.CurrentPlayer.Id);
        }

        Console.WriteLine("---------------");
    }
}