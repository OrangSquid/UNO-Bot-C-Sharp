using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UNODiscordBot.Exceptions;
using UNODiscordBot.Wrappers;
using UNOLib;
using UNOLib.Cards;
using UNOLib.Exceptions;
using UNOLib.Player;

namespace UNODiscordBot;

public class UnoSlashCommands : ApplicationCommandModule
{
    private UnoLibWrapper Uno { get; }

    public UnoSlashCommands(IServiceProvider serviceProvider)
    {
        Uno = serviceProvider.GetRequiredService<UnoLibWrapper>();
    }

    [SlashCommandGroup("settings", "Change the way the game plays")]
    public class SettingsCommands : ApplicationCommandModule
    {
        private UnoLibWrapper Uno { get; }

        public SettingsCommands(IServiceProvider serviceProvider)
        {
            Uno = serviceProvider.GetRequiredService<UnoLibWrapper>();
        }

        [SlashCommand("drawUntilPlayableCard", "Draw Until Playable Card")]
        public async Task DrawUntilPlayableCardCommand(InteractionContext ctx,
            [Option("value", "Draw Until Playable Card")]
            string drawUntilPlayableCard)
        {
            try
            {
                Uno.SetDrawUntilPlayableCard(ctx.Channel.Id, Convert.ToBoolean(drawUntilPlayableCard));
                await ctx.CreateResponseAsync($"Draw Until Playable Card set to {drawUntilPlayableCard}");
            }
            catch (ArgumentException)
            {
                await ctx.CreateResponseAsync("Something went very wrong", true);
            }
        }

        [SlashCommand("jumpIn", "Jump In")]
        public async Task JumpInCommand(InteractionContext ctx,
            [Option("value", "Jump In")] string jumpIn)
        {
            try
            {
                Uno.SetJumpIn(ctx.Channel.Id, Convert.ToBoolean(jumpIn));
                await ctx.CreateResponseAsync($"Jump In set to {jumpIn}");
            }
            catch (ArgumentException)
            {
                await ctx.CreateResponseAsync("Something went very wrong", true);
            }
        }

        [SlashCommand("mustPlay", "Must Play")]
        public async Task MustPlayCommand(InteractionContext ctx,
            [Option("value", "Must Play")] bool mustPlay)
        {
            try
            {
                Uno.SetMustPlay(ctx.Channel.Id, Convert.ToBoolean(mustPlay));
                await ctx.CreateResponseAsync($"Must Play set to {mustPlay}");
            }
            catch (ArgumentException)
            {
                await ctx.CreateResponseAsync("Something went very wrong", true);
            }
        }

        [SlashCommand("stackPlusTwo", "Stack Plus Two Cards")]
        public async Task StackPlusTwoCommand(InteractionContext ctx,
            [Option("value", "Stack Plus Two Cards")] bool stackPlusTwo)
        {
            try
            {
                Uno.SetStackPlusTwo(ctx.Channel.Id, Convert.ToBoolean(stackPlusTwo));
                await ctx.CreateResponseAsync($"Stack Plus Two set to {stackPlusTwo}");
            }
            catch (ArgumentException)
            {
                await ctx.CreateResponseAsync("Something went very wrong", true);
            }
        }

        [SlashCommand("unoPenalty", "Not saying uno penalty")]
        public async Task UnoPenaltyCommand(InteractionContext ctx,
            [Option("value", "Not saying uno penalty")]
            long unoPenalty)
        {
            try
            {
                Uno.SetUnoPenalty(ctx.Channel.Id, Convert.ToInt32(unoPenalty));
                await ctx.CreateResponseAsync($"Uno Penalty set to {unoPenalty}");
            }
            catch (ArgumentException)
            {
                await ctx.CreateResponseAsync("Something went very wrong", true);
            }
        }
    }

    [SlashCommand("new", "Make a new game for people to join")]
    public async Task NewGameCommand(InteractionContext ctx)
    {
        try
        {
            Uno.CreateGame(ctx.Channel.Id, ctx.User);
            await ctx.CreateResponseAsync("Lobby Created");
        }
        catch (GameAlreadyExistsException)
        {
            await ctx.CreateResponseAsync("Game already exists", true);
        }
    }

    [SlashCommand("join", "Join an existing game")]
    public async Task JoinCommand(InteractionContext ctx)
    {
        try
        {
            Uno.JoinGame(ctx.Channel.Id, ctx.User);
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new()
            {
                Content = ctx.User.Username + " joined the lobby"
            });
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (AlreadyPartOfTheGameException)
        {
            await ctx.CreateResponseAsync("Already part of the game", true);
        }
        catch (TooManyPlayersException)
        {
            await ctx.CreateResponseAsync("Too many players", true);
        }
    }

    [SlashCommand("leave", "Leave an existing game")]
    public async Task LeaveCommand(InteractionContext ctx)
    {
        try
        {
            Uno.LeaveGame(ctx.Channel.Id, ctx.User);
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new()
            {
                Content = ctx.User.Username + " left the lobby"
            });
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Lobby does not exist", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
        catch (NotEnoughPlayersException)
        {
            await ctx.CreateResponseAsync("Not enough players, lobby was deleted");
        }
    }

    [SlashCommand("start", "Starts the game")]
    public async Task StartCommand(InteractionContext ctx)
    {
        try
        {
            var state = Uno.StartGame(ctx.Channel.Id, ctx.User);
            await StateInterpreter(true, state, ctx);
        }
        catch (GameAlreadyStartedException)
        {
            await ctx.CreateResponseAsync("Game has already started", true);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
        catch (NotEnoughPlayersException)
        {
            await ctx.CreateResponseAsync("Not enough players", true);
        }
    }

    [SlashCommand("play", "Plays a given card")]
    public async Task PlayCardCommand(InteractionContext ctx,
        [Option("card", "The card to play", true)]
        [Autocomplete(typeof(CardAutocompleteProvider))] string card)
    {
        try
        {
            var state = Uno.PlayCard(ctx.Channel.Id, ctx.User, card);
            await StateInterpreter(false, state, ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
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
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

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
            var state = Uno.ChangeColor(ctx.Channel.Id, ctx.User, color);
            await StateInterpreter(false, state, ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (CannotChangeColorException)
        {
            await ctx.CreateResponseAsync("Cannot change color right now", true);
        }
        catch (NotPlayersTurnException)
        {
            await ctx.CreateResponseAsync("It's not your turn", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    [SlashCommand("draw", "Draws a card by the chosen rule set")]
    public async Task DrawCardCommand(InteractionContext ctx)
    {
        try
        {
            await StateInterpreter(false, Uno.DrawCard(ctx.Channel.Id, ctx.User), ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (GameIsFinishedException)
        {
            await ctx.CreateResponseAsync("Game has already finished", true);
        }
        catch (NotPlayersTurnException)
        {
            await ctx.CreateResponseAsync("It's not your turn", true);
        }
        catch (PlayerCannotDrawException)
        {
            await ctx.CreateResponseAsync("You cannot draw anymore", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    [SlashCommand("check", "Shows your current deck")]
    public async Task CheckCommand(InteractionContext ctx)
    {
        try
        {
            var message = new StringBuilder();
            var users = Uno.GetGame(ctx.Channel.Id);
            var checkingPlayer = Uno.GetPlayer(ctx.Channel.Id, ctx.User);

            // Create string with all the players cards
            foreach (var user in users)
            {
                message.Append($"{((DiscordPlayer)user).User.Username}: ");
                message.Append(UnoMessageBuilder.PlayerHandToBackEmoji(user));
                message.Append('\n');
            }
            message.Append("Here's your current deck:\n");
            await ctx.CreateResponseAsync(message.ToString(), true);

            // Shows the cards the player has
            message.Clear();
            message.Append(UnoMessageBuilder.PlayerHandToFrontEmoji(checkingPlayer));
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(message.ToString()).AsEphemeral());
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    [SlashCommand("skip", "Skips your turn after drawing a card")]
    public async Task SkipCommand(InteractionContext ctx)
    {
        try
        {
            await StateInterpreter(false, Uno.Skip(ctx.Channel.Id, ctx.User), ctx);
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (GameIsFinishedException)
        {
            await ctx.CreateResponseAsync("Game has already finished", true);
        }
        catch (NotPlayersTurnException)
        {
            await ctx.CreateResponseAsync("It's not your turn", true);
        }
        catch (PlayerCannotSkipException)
        {
            await ctx.CreateResponseAsync("You cannot skip", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    [SlashCommand("End", "Ends the game abruptly")]
    public async Task KillCommand(InteractionContext ctx)
    {
        try
        {
            Uno.EndGame(ctx.Channel.Id, ctx.User);
            await ctx.CreateResponseAsync("Game finished");
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game does not exist", true);
        }
        catch (PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    //TODO refactor this shit
    private async Task StateInterpreter(bool newGame, GameState state, BaseContext ctx)
    {
        var message = new StringBuilder();
        var fieldTitle = new StringBuilder();
        var fieldValue = new StringBuilder();
        DiscordEmbedBuilder embedMessage = new();

        if (newGame)
        {
            embedMessage.WithTitle("Game started\n");
            message.Append($"Card on table: {state.OnTable}\n");
        }
        else
        {
            var authorTitle = $"{((DiscordPlayer)state.PreviousPlayer!).User.Username}'s turn";
            // Player JumpedIn
            if (state is { JumpedIn: true, PreviousPlayer: not null })
            {
                authorTitle = $"{((DiscordPlayer)state.PreviousPlayer).User.Username} jumped in!";
            }
            // Played a card
            if (state.CardsPlayed.Count != 0 && state.PreviousPlayer != null)
            {
                if (!state.JumpedIn)
                    message.Append($"{((DiscordPlayer)state.PreviousPlayer).User.Username} played:\n");

                embedMessage.WithAuthor(authorTitle, null, ctx.User.AvatarUrl);
                foreach (ICard card in state.CardsPlayed)
                {
                    message.Append(card);
                    message.Append("\n");
                }
                message.Append("\n");
                message.Append($"{((DiscordPlayer)state.PreviousPlayer).User.Username} card(s):\n");
                message.Append(UnoMessageBuilder.PlayerHandToBackEmoji(state.PreviousPlayer));
                message.Append("\n\n");
            }
            // Cards were drawn
            if (state.WhoDrewCards != null)
            {
                authorTitle = $"drew {state.CardsDrawn} card(s)\n";
                string authorImgUrl = state.OnTable is WildCard { Symbol: WildCardSymbols.PlusFour } or ColorCard { Symbol: ColorCardSymbols.PlusTwo } ? ((DiscordPlayer)state.WhoDrewCards).User.AvatarUrl : ctx.User.AvatarUrl;
                embedMessage.WithAuthor(authorTitle, null, authorImgUrl);

                fieldTitle.Append($"{((DiscordPlayer)state.WhoDrewCards).User.Username}'s card(s):\n");
                fieldValue.Append(UnoMessageBuilder.PlayerHandToBackEmoji(state.WhoDrewCards));
                embedMessage.AddField(fieldTitle.ToString(), fieldValue.ToString());
                fieldTitle.Clear();
                fieldValue.Clear();
            }
            // Players were skipped
            if (state.PlayersSkipped.Count != 0)
            {
                message.Append("These Players were skipped: \n");
                foreach (IPlayer player in state.PlayersSkipped)
                {
                    message.Append($"   Player {((DiscordPlayer)player).User.Username}\n");
                }
                message.Append("\n");
            }
            // The order was reversed
            if (state.JustReversedOrder)
            {
                message.Append("The order has been reversed!\n");
            }
            // Waiting for the color to change
            if (state.WaitingOnColorChange)
            {
                message.Append($"Waiting for Player {((DiscordPlayer)state.CurrentPlayer).User.Username} to choose a color...\n");
            }
            if (state.ColorChanged != null)
            {
                message.Append($"Color changed to: {state.ColorChanged}\n");
            }
            if (state is { HasSkipped: true, PreviousPlayer: not null })
            {
                message.Append($"Player {((DiscordPlayer)state.PreviousPlayer).User.Username} has skipped their turn\n");
            }
            if (state.GameFinished)
            {
                message.Append("Game has ended\n");
            }
        }

        if (state.NewTurn)
        {
            fieldTitle.Append($"Your turn now: {((DiscordPlayer)state.CurrentPlayer).User.Username}\n");
            fieldValue.Append(UnoMessageBuilder.PlayerHandToBackEmoji(state.CurrentPlayer));
            embedMessage.AddField(fieldTitle.ToString(), fieldValue.ToString());
        }
        
        
        await ctx.CreateResponseAsync(embedMessage
            .WithThumbnail(((ICardWrapper)state.OnTable).Url)
            .WithDescription(message.ToString())
            .WithTimestamp(DateTime.Now)
            .WithColor(((ICardWrapper)state.OnTable).DiscordColor)
            );
    }
}