using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using UNODiscordBot.Exceptions;
using UNODiscordBot.Wrappers;
using UNOLib;
using UNOLib.Cards;
using UNOLib.Exceptions;
using UNOLib.Player;

namespace UNODiscordBot;

public class UnoSlashCommands : ApplicationCommandModule
{
#pragma warning disable CS8618
    public UnoLibWrapper Uno { get; set; }
    public UnoMessageBuilder MessageBuilder { get; set; }

    [SlashCommandGroup("settings", "Change the way the game plays")]
    public class SettingsCommands : ApplicationCommandModule
    {
        public UnoLibWrapper Uno { get; set; }
#pragma warning restore CS8618

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
            var users = Uno.GetDiscordUsers(ctx.Channel.Id);
            var checkingPlayer = Uno.GetPlayer(ctx.Channel.Id, ctx.User);

            // Create string with all the players cards
            foreach (var user in users)
            {
                var otherPlayer = Uno.GetPlayer(ctx.Channel.Id, user);

                message.Append($"{user.Username}: ");
                message.Append(MessageBuilder.PlayerHandToBackEmoji(otherPlayer));
                message.Append('\n');
            }
            message.Append("Here's your current deck:\n");
            await ctx.CreateResponseAsync(message.ToString(), true);

            // Shows the cards the player has
            message.Clear();
            message.Append(MessageBuilder.PlayerHandToFrontEmoji(checkingPlayer));
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
            Uno.EndGame(ctx.Channel.Id);
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
        string message = "";
        string authorImgUrl = "";
        string fieldTitle = "";
        string fieldValue = "";
        string imageUrl = "https://raw.githubusercontent.com/OrangSquid/UNO-Bot-C-Sharp/discord_bot/deck/";
        DiscordEmbedBuilder embedMessage = new();

        if (newGame)
        {
            embedMessage.WithTitle("Game started\n");
            message += $"Card on table: {state.OnTable}\n";
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
                    message += $"{((DiscordPlayer)state.PreviousPlayer).User.Username} played:\n";

                embedMessage.WithAuthor(authorTitle, null, ctx.User.AvatarUrl);
                foreach (ICard card in state.CardsPlayed)
                {
                    message += card;
                    message += "\n";
                }
                message += "\n";
                message += $"{((DiscordPlayer)state.PreviousPlayer).User.Username} card(s):\n";
                message += MessageBuilder.PlayerHandToBackEmoji(state.PreviousPlayer);
                message += "\n\n";
            }
            // Cards were drawn
            if (state.WhoDrewCards != null)
            {
                authorTitle = $"drew {state.CardsDrawn} card(s)\n";
                authorImgUrl += ctx.User.AvatarUrl;
                if (state.OnTable is WildCard { Symbol: WildCardSymbols.PlusFour } or ColorCard { Symbol: ColorCardSymbols.PlusTwo })
                {

                    var users = Uno.GetDiscordUsers(ctx.Channel.Id);
                    var whoDrewCards = users.Find(user => users.IndexOf(user) == state.WhoDrewCards.Id);
                    authorImgUrl = whoDrewCards!.AvatarUrl;
                }
                embedMessage.WithAuthor(authorTitle, null, authorImgUrl);

                fieldTitle += $"{((DiscordPlayer)state.WhoDrewCards).User.Username}'s card(s):\n";
                fieldValue += MessageBuilder.PlayerHandToBackEmoji(state.WhoDrewCards);
                embedMessage.AddField(fieldTitle, fieldValue);
                fieldTitle = "";
                fieldValue = "";
            }
            // Players were skipped
            if (state.PlayersSkipped.Count != 0)
            {
                message += "These Players were skipped: \n";
                foreach (IPlayer player in state.PlayersSkipped)
                {
                    message += $"   Player {((DiscordPlayer)player).User.Username}\n";
                }
                message += "\n";
            }
            // The order was reversed
            if (state.JustReversedOrder)
            {
                message += "The order has been reversed!\n";
            }
            // Waiting for the color to change
            if (state.WaitingOnColorChange)
            {
                message += $"Waiting for Player {((DiscordPlayer)state.CurrentPlayer).User.Username} to choose a color...\n";
            }
            if (state.ColorChanged != null)
            {
                message += $"Color changed to: {state.ColorChanged}\n";
            }
            if (state is { HasSkipped: true, PreviousPlayer: { } })
            {
                message += $"Player {((DiscordPlayer)state.PreviousPlayer).User.Username} has skipped their turn\n";
            }
            if (state.GameFinished)
            {
                message += "Game has ended\n";
            }
        }

        if (state.NewTurn)
        {
            fieldTitle += $"Your turn now: {((DiscordPlayer)state.CurrentPlayer).User.Username}\n";
            fieldValue += MessageBuilder.PlayerHandToBackEmoji(state.CurrentPlayer);
            embedMessage.AddField(fieldTitle, fieldValue);
        }

        string colorHex;
        if (state.WaitingOnColorChange)
        {
            colorHex = "#000000";
        }
        else
        {
            colorHex = state.OnTable.Color switch
            {
                CardColors.Red => "#FF5555",
                CardColors.Green => "#55AA55",
                CardColors.Yellow => "#FFAA00",
                CardColors.Blue => "#5555FF",
                _ => "#000000",
            };
        }

        imageUrl += CardUrl(state);
        await ctx.CreateResponseAsync(embedMessage
            .WithThumbnail(imageUrl)
            .WithDescription(message)
            .WithTimestamp(DateTime.Now)
            .WithColor(new DiscordColor(colorHex))
            );
    }

    private static string CardUrl(GameState state)
    {
        var card = state.OnTable;
        var cardImg = "";

        switch (card)
        {
            case WildCard wc:
                {
                    if (!state.WaitingOnColorChange)
                    {
                        cardImg += state.ColorChanged.ToString()!.ToLower();
                        cardImg += "%20";
                    }
                    return cardImg + "wild%20" + wc.Symbol.ToString().ToLower() + ".png";
                }
            case ColorCard cc:
                {
                    cardImg += cc.Color.ToString().ToLower() + "%20";

                    if (cc.Symbol.Equals(ColorCardSymbols.Reverse) || cc.Symbol.Equals(ColorCardSymbols.PlusTwo) || cc.Symbol.Equals(ColorCardSymbols.Skip))
                        cardImg += cc.Symbol.ToString().ToLower();
                    else
                        cardImg += Enum.Format(typeof(ColorCardSymbols), cc.Symbol, "d");
                    cardImg += ".png";

                    return cardImg;
                }
            default:
                return "";
        }
    }
}