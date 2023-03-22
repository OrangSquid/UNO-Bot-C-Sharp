using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

namespace UNODiscordBot;

public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Main/Dev");
        string choice = Console.ReadLine() ?? string.Empty;
        string token = File.ReadAllText(choice == "m" ? "MToken.txt" : "DToken.txt");

        DiscordClient discord = new(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });

        ServiceProvider services = new ServiceCollection()
            .AddSingleton<UnoLibWrapper>()
            .BuildServiceProvider();

        SlashCommandsExtension slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
        {
            Services = services
        });

        slash.RegisterCommands<UnoSlashCommands>(556652655397830657);
        slash.RegisterCommands<UnoSlashCommands>(1088175511320145970);


        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}