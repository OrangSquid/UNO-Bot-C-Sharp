using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UNODiscordBot.Wrappers;

namespace UNODiscordBot;

public static class Program
{
    private static UnoMessageBuilder? _messageBuilder;

    public static async Task Main()
    {
        Console.WriteLine(@"Main/Dev");
        string choice = Console.ReadLine() ?? string.Empty;
        string token = await File.ReadAllTextAsync(choice == "m" ? "MToken.txt" : "DToken.txt");

        DiscordClient discord = new(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });

        _messageBuilder = new UnoMessageBuilder();

        var services = new ServiceCollection()
            .AddSingleton<UnoLibWrapper>()
            .AddSingleton(_messageBuilder)
            .BuildServiceProvider();

        var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
        {
            Services = services
        });

        slash.RegisterCommands<UnoSlashCommands>(556652655397830657);
        slash.RegisterCommands<UnoSlashCommands>(1088175511320145970);
        slash.RegisterCommands<UnoSlashCommands>(939964810521628752);


        discord.GuildDownloadCompleted += DiscordOnGuildDownloadCompleted;

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }

    private static Task DiscordOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        _messageBuilder?.BuildEmojiDictionary(sender);
        return Task.CompletedTask;
    }
}