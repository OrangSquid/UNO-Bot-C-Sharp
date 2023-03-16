using DSharpPlus.SlashCommands;
using UNODiscordBot.Exceptions;
using UNOLib;
using static UNODiscordBot.UNOLibWrapper;

namespace UNODiscordBot;

public class UNOSlashCommands : ApplicationCommandModule
{
    public UNOLibWrapper Uno { private get; set; }

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
            await ctx.CreateResponseAsync("Game Already Exists");
        }
    }

    [SlashCommand("join", "Join an existing game")]
    public async Task JoinCommand(InteractionContext ctx)
    {
        try
        {
            Uno.JoinGame(ctx.Guild.Id, ctx.User);
            await ctx.CreateResponseAsync("{ctx.User.ToString} Joined Lobby");
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist");
        }
        catch (AlreadyPartOfTheGameException)
        {
            await ctx.CreateResponseAsync("Already part of the game");
        }
    }

    [SlashCommand("start", "Starts the game")]
    public async Task StartCommand(InteractionContext ctx)
    {
        try
        {
            Uno.StartGame(ctx.Guild.Id);
            await ctx.CreateResponseAsync("Game Started");
        }
        catch (GameAlreadyStartedException)
        {
            await ctx.CreateResponseAsync("Game has already started");
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist");
        }
    }

    //TODO
    [SlashCommand("play", "Plays a given card")]
    public async Task PlayCardCommand(InteractionContext ctx, [Option("card", "a", true)][Autocomplete(typeof(CardAutocompleteProvider))] string card)
    {
        try
        {
            
            await ctx.CreateResponseAsync($"{ctx.User.ToString} Joined Lobby");
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist");
        }
    }

    //TODO
    [SlashCommand("choose", "Choose the color after playing a Wild Card")]
    public async Task ChooseColorCommand(InteractionContext ctx)
    {
        try
        {
            
            await ctx.CreateResponseAsync("{ctx.User.ToString} Joined Lobby");
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist");
        }
    }

    //TODO
    [SlashCommand("draw", "Draws a card by the chosen rule set")]
    public async Task DrawCardCommand(InteractionContext ctx)
    {
        try
        {
            Uno.JoinGame(ctx.Guild.Id, ctx.User);
            await ctx.CreateResponseAsync("{ctx.User.ToString} Joined Lobby");
        }
        catch (GameDoesNotExistException)
        {
            await ctx.CreateResponseAsync("Game Does Not Exist");
        }
    }

    [SlashCommand("check", "Shows your current deck")]
    public async Task CheckCommand(InteractionContext ctx)
    {
        try
        {
            IPlayer player = Uno.CheckCards(ctx.Guild.Id, ctx.User);
            string message = "Here's your current deck:\n";
            foreach(ICard card in player)
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
        catch(PlayerDoesNotExistException)
        {
            await ctx.CreateResponseAsync("You're not part of the game", true);
        }
    }

    private void StateInterpreter()
    {

    }
}
