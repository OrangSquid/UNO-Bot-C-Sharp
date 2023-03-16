using DSharpPlus.SlashCommands;
using UNODiscordBot.Exceptions;

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
            await ctx.CreateResponseAsync("Joined Lobby");
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

    private void StateInterpreter()
    {

    }
}
