using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

namespace UNODiscordBot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Main/Dev");
        string choice = Console.ReadLine();
        string token;
        if (choice == "m")
            token = File.ReadAllText("MToken.txt");
        else
            token = File.ReadAllText("DToken.txt");

        DiscordClient discord = new(new DiscordConfiguration()
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });

        ServiceProvider services = new ServiceCollection()
            .AddSingleton<UNOLibWrapper>()
            .BuildServiceProvider();

        SlashCommandsExtension slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
        {
            Services = services
        });

        slash.RegisterCommands<UNOSlashCommands>(556652655397830657);

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}