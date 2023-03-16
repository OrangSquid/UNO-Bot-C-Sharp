using DSharpPlus.SlashCommands;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Exceptions;

namespace UNODiscordBot;

public class UNOSlashCommands : ApplicationCommandModule
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UNOLibWrapper Uno { private get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SlashCommand("new", "Make a new game for people to join")]
    public async Task NewGameCommand(InteractionContext ctx)
    {
        try
        {
            Uno.CreateGame(ctx.Guild.Id, ctx.User);
            await ctx.CreateResponseAsync("Lobby Created");
        }
        catch (GameAlreadyExistsException)
        {
            await ctx.CreateResponseAsync("Game Already Exists", true);
        }
    }

    [SlashCommand("join", "Join an existing game")]
    public async Task JoinCommand(InteractionContext ctx)
    {
        try
        {
            Uno.JoinGame(ctx.Guild.Id, ctx.User);
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new()
            {
                Content = ctx.User.Username + " Joined Lobby"
            });
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist", true);
        }
        catch (AlreadyPartOfTheGameException)
        {
            await ctx.CreateResponseAsync("Already part of the game", true);
        }
    }

    [SlashCommand("start", "Starts the game")]
    public async Task StartCommand(InteractionContext ctx)
    {
        try
        {
            GameState state = Uno.StartGame(ctx.Guild.Id);
            await StateInterpreter(true, state, ctx);
        }
        catch (GameAlreadyStartedException)
        {
            await ctx.CreateResponseAsync("Game has already started", true);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist", true);
        }
    }

    //TODO
    [SlashCommand("play", "Plays a given card")]
    public async Task PlayCardCommand(InteractionContext ctx,
        [Option("card", "The card to play", true)]
        [Autocomplete(typeof(CardAutocompleteProvider))] string card)
    {
        try
        {
            GameState state = Uno.PlayCard(ctx.Guild.Id, ctx.User, card);
            await StateInterpreter(false, state, ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist", true);
        }
        catch (GameIsFinishedException)
        {
            await ctx.CreateResponseAsync("Game has finished", true);
        }
        catch (CardCannotBePlayedException)
        {
            await ctx.CreateResponseAsync("Card cannot be played", true);
        }
        catch (NotPlayersTurnException)
        {
            await ctx.CreateResponseAsync("It's not your turn", true);
        }
    }

    //TODO
    [SlashCommand("choose", "Choose the color after playing a Wild Card")]
    public async Task ChooseColorCommand(InteractionContext ctx,
        [Option("color", "The color to change the wild card you played")]
        [Choice("Blue", "Blue")]
        [Choice("Green", "Green")]
        [Choice("Red", "Red")]
        [Choice("Yellow", "Yellow")] string color)
    {
        try
        {
            GameState state = Uno.ChangeColor(ctx.Guild.Id, ctx.User, color);
            await StateInterpreter(false, state, ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist", true);
        }
        catch (CannotChangeColorException)
        {
            await ctx.CreateResponseAsync("Cannot change color right now", true);
        }
        catch (NotPlayersTurnException)
        {
            await ctx.CreateResponseAsync("It's not your turn", true);
        }
    }

    //TODO
    [SlashCommand("draw", "Draws a card by the chosen rule set")]
    public async Task DrawCardCommand(InteractionContext ctx)
    {
        try
        {
            await StateInterpreter(false, Uno.DrawCard(ctx.Guild.Id, ctx.User), ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist", true);
        }
        catch (GameIsFinishedException)
        {
            await ctx.CreateResponseAsync("Game has already finished", true);
        }
        catch (NotPlayersTurnException)
        {
            await ctx.CreateResponseAsync("It's not your turn", true);
        }
    }

    [SlashCommand("check", "Shows your current deck")]
    public async Task CheckCommand(InteractionContext ctx)
    {
        try
        {
            IPlayer player = Uno.CheckCards(ctx.Guild.Id, ctx.User);
            string message = "Here's your current deck:\n";
            foreach (ICard card in player)
            {

                message += card.ToString();
                message += "\n";
            }
            await ctx.CreateResponseAsync(message, true);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    private async Task StateInterpreter(bool newGame, GameState state, InteractionContext ctx)
    {
        string message = "---------------\n";

        if (newGame)
        {
            message += "Game started\n";
            message += $"Card on table: {state.OnTable}\n";
        }
        else
        {
            // Played a card
            if (state.CardsPlayed.Count != 0 && state.PreviousPlayer != null)
            {
                message += $"Player {Uno.GetUser(ctx.Guild.Id, state.PreviousPlayer.Id).Username} played:\n";
                foreach (ICard card in state.CardsPlayed)
                {
                    message += card;
                    message += "\n";
                }

            }
            // Cards were drawn
            if (state.WhoDrewCards != null)
            {
                message += $"Player {Uno.GetUser(ctx.Guild.Id, state.WhoDrewCards.Id).Username} drew {state.CardsDrawn} cards\n";
            }
            // Players were skipped
            if (state.PlayersSkiped.Count != 0)
            {
                message += "These Players were skipped: \n";
                foreach (IPlayer player in state.PlayersSkiped)
                {
                    message += $"Player {Uno.GetUser(ctx.Guild.Id, player.Id).Username}\n";
                }
            }
            // The order was reversed
            if (state.JustReversedOrder)
            {
                message += "The order has been reversed\n";
            }
            // Waiting for the color to change
            if (state.WaitingOnColorChange)
            {
                message += $"Waiting for Player {Uno.GetUser(ctx.Guild.Id, state.CurrentPlayer.Id).Username} to choose a color\n";
            }
            if (state.ColorChanged != null)
            {
                message += $"Color changed to: {state.ColorChanged}\n";
            }
            if (state.GameFinished)
            {
                message += "Game has ended\n";
            }
        }
        if (state.NewTurn)
        {
            message += $"Your turn now: {Uno.GetUser(ctx.Guild.Id, state.CurrentPlayer.Id).Username}\n";
        }

        message += "---------------";

        await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new()
        {
            Content = message
        });
    }
}
