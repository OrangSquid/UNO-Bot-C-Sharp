using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace UNODiscordBot;

public class Program
{
    private DiscordSocketClient? _client;
    private InteractionService? _interactionService;

    public static Task Main() => new Program().MainAsync();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;
        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += SlashCommandHandler;

        _interactionService = new(_client.Rest);
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), null);

        var token = File.ReadAllText("Token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public async Task Client_Ready()
    {
        Console.WriteLine("i");
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        await command.RespondAsync("lol");
    }
}