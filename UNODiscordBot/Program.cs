using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

namespace UNODiscordBot;

public class Program
{
    public static async Task Main()
    {
        string token = File.ReadAllText("Token.txt");

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