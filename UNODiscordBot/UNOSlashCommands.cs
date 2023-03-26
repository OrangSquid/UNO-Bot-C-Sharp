using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Cards;
using UNOLib.Exceptions;
using UNOLib.Player;

namespace UNODiscordBot;

public class UnoSlashCommands : ApplicationCommandModule
{
   private UnoLibWrapper Uno { get; set; }

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
            var state = Uno.StartGame(ctx.Guild.Id);
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

    [SlashCommand("play", "Plays a given card")]
    public async Task PlayCardCommand(InteractionContext ctx,
        [Option("card", "The card to play", true)]
        [Autocomplete(typeof(CardAutocompleteProvider))] string card)
    {
        try
        {
            var state = Uno.PlayCard(ctx.Guild.Id, ctx.User, card);
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
            var state = Uno.ChangeColor(ctx.Guild.Id, ctx.User, color);
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
        catch (PlayerCannotDrawException)
        {
            await ctx.CreateResponseAsync("You cannot draw anymore", true);
        }
    }


    [SlashCommand("check", "Shows your current deck")]
    public async Task CheckCommand(InteractionContext ctx)
    {
        try
        {
            DiscordEmoji emoji;
            List<DiscordUser> users = Uno.GetDiscordUsers(ctx.Guild.Id);
            var checkingPlayer = Uno.CheckCards(ctx.Guild.Id, ctx.User);
            var message = "Here's your current deck:\n";

            await ctx.CreateResponseAsync(message, true);
            message = "";

            //Shows the ctx.user deck
            foreach (var card in checkingPlayer)
            {
                emoji = DiscordEmoji.FromGuildEmote(ctx.Client, UNOMessageBuilder.emojiIds.GetValueOrDefault(card.ToString()));
                message += emoji;
            }
            await ctx.CreateResponseAsync(message, true);
            message = "";
            
            UNOMessageBuilder.emojiIds.TryGetValue("BackCard", out ulong id);
            emoji = DiscordEmoji.FromGuildEmote(ctx.Client, id);
            //Shows how many cards all the players has
            foreach (DiscordUser user in users)
            {
                IPlayer otherPlayer = Uno.CheckCards(ctx.Guild.Id, ctx.User);
                if (!otherPlayer.Equals(checkingPlayer))
                {
                    message += $"{user.Username}:";
                    for (int i = 0; i < otherPlayer.NumCards; i++)
                    {
                        message += emoji;
                    }
                }
            }
            
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

    [SlashCommand("skip", "Skips your turn after drawing a card")]
    public async Task SkipCommand(InteractionContext ctx)
    {
        try
        {
            await StateInterpreter(false, Uno.Skip(ctx.Guild.Id, ctx.User), ctx);
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
        catch (PlayerCannotSkipException)
        {
            await ctx.CreateResponseAsync("You cannot skip", true);
        }
    }


    //TODO refactor this shi
    private async Task StateInterpreter(bool newGame, GameState state, InteractionContext ctx)
    {
        string message = "";
        string author = "";
        string colorHex = "";
        string fieldTitle = "";
        string fieldValue = "";
        string imageURL = "https://raw.githubusercontent.com/OrangSquid/UNO-Bot-C-Sharp/discord_bot/deck/";
        DiscordEmbedBuilder embedMessage = new DiscordEmbedBuilder();
        DiscordEmoji emoji;




        if (newGame)
        {
            embedMessage.WithTitle("Game started\n");
            message += $"Card on table: {state.OnTable}\n";
        }
        else
        {
            // Player JumpedIn
            if (state is { JumpedIn: true, PreviousPlayer: { } })
            {
                author += $"{Uno.GetUser(ctx.Guild.Id, state.PreviousPlayer.Id).Username} jumped in!";
                embedMessage.WithAuthor(author, null, ctx.User.AvatarUrl);
                author = "";
            }
            // Played a card
            if (state.CardsPlayed.Count != 0 && state.PreviousPlayer != null)
            {
                if(!state.JumpedIn)
                    author += $"{Uno.GetUser(ctx.Guild.Id, state.PreviousPlayer.Id).Username} played:\n";
                
                embedMessage.WithAuthor(author, null, ctx.User.AvatarUrl);
                foreach (ICard card in state.CardsPlayed)
                {
                    message += card;
                    message += "\n";
                }
                message += $"{Uno.GetUser(ctx.Guild.Id, state.PreviousPlayer.Id).Username} card(s):\n";
                emoji = DiscordEmoji.FromGuildEmote(ctx.Client, UNOMessageBuilder.emojiIds.GetValueOrDefault("BackCard"));
                for (int i = 0; i < state.PreviousPlayer.NumCards; i++)
                    message += emoji;
                message += "\n";
                author = "";
            }
            // Cards were drawn
            if (state.WhoDrewCards != null)
            {
                author += $"drew {state.CardsDrawn} card(s)\n";
                embedMessage.WithAuthor(author, null, ctx.User.AvatarUrl);

                fieldTitle += $"{Uno.GetUser(ctx.Guild.Id, state.WhoDrewCards.Id).Username}'s card(s):\n";
                emoji = DiscordEmoji.FromGuildEmote(ctx.Client, UNOMessageBuilder.emojiIds.GetValueOrDefault("BackCard"));
                for (int i = 0; i < state.WhoDrewCards.NumCards; i++)
                    fieldValue += emoji;
                embedMessage.AddField(fieldTitle, fieldValue, false);
                fieldTitle = "";
                fieldValue = "";
            }
            // Players were skipped
            if (state.PlayersSkipped.Count != 0)
            {
                message += "These Players were skipped: \n";
                foreach (IPlayer player in state.PlayersSkipped)
                {
                    message += $"   Player {Uno.GetUser(ctx.Guild.Id, player.Id).Username}\n";
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
                message += $"Waiting for Player {Uno.GetUser(ctx.Guild.Id, state.CurrentPlayer.Id).Username} to choose a color...\n";
            }
            if (state.ColorChanged != null)
            {
                message += $"Color changed to: {state.ColorChanged}\n";
            }
            if (state is { HasSkipped: true, PreviousPlayer: { } })
            {
                message += $"Player {Uno.GetUser(ctx.Guild.Id, state.PreviousPlayer.Id).Username} has skipped their turn\n";
            }
            if (state.GameFinished)
            {
                message += "Game has ended\n";
            }
        }

        if (state.NewTurn)
        {

            fieldTitle += $"Your turn now: {Uno.GetUser(ctx.Guild.Id, state.CurrentPlayer.Id).Username}\n";

            emoji = DiscordEmoji.FromGuildEmote(ctx.Client, 746444081424760943);
            for (int i = 0; i < state.CurrentPlayer.NumCards; i++)
                fieldValue += emoji;
            embedMessage.AddField(fieldTitle, fieldValue, false);


        }

        if (state.WaitingOnColorChange)
            colorHex += "#000000";
        else
            switch (state.OnTable.Color)
            {
                case CardColors.Red:
                    colorHex += "#FF5555";
                    break;
                case CardColors.Green:
                    colorHex += "#55AA55";
                    break;
                case CardColors.Yellow:
                    colorHex += "#FFAA00";
                    break;
                case CardColors.Blue:
                    colorHex += "#5555FF";
                    break;
                default:
                    colorHex += "#000000";
                    break;
            }



        imageURL += CardURL(state);
        await ctx.CreateResponseAsync(embedMessage
            .WithThumbnail(imageURL)
            .WithDescription(message)
            .WithTimestamp(DateTime.Now)
            .WithColor(new DiscordColor(colorHex))
            );
    }

    private static string CardURL(GameState state)
    {
        ICard card = state.OnTable;
        string cardImg = "";

        if (card is WildCard wc)
        {
            if (!state.WaitingOnColorChange)
            {
                cardImg += state.ColorChanged.ToString().ToLower();
                cardImg += "%20";
            }
            return "wild%20" + wc.Symbol.ToString().ToLower() + ".png";
        }
        else if (card is ColorCard cc)
        {
            cardImg += cc.Color.ToString().ToLower() + "%20";

            if (cc.Symbol.Equals(ColorCardSymbols.Reverse) || cc.Symbol.Equals(ColorCardSymbols.PlusTwo) || cc.Symbol.Equals(ColorCardSymbols.Skip))
                cardImg += cc.Symbol.ToString().ToLower();
            else
                cardImg += Enum.Format(typeof(ColorCardSymbols), cc.Symbol, "d");
            cardImg += ".png";

            return cardImg;
        }

        return "";
    }
}