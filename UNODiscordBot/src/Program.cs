using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UNODiscordBot.Wrappers;

namespace UNODiscordBot;

public static class Program
{
    private static UnoMessageBuilder? _messageBuilder;
    private static UnoLibWrapper? _unoLibWrapper;

    public static async Task Main()
    {
        string token = Environment.GetEnvironmentVariable("UNO_TOKEN") ?? throw new Exception("No token found");

        DiscordClient discord = new(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });

        _messageBuilder = new UnoMessageBuilder();
        _unoLibWrapper = new UnoLibWrapper();

        var services = new ServiceCollection()
            .AddSingleton(_unoLibWrapper)
            .AddSingleton(_messageBuilder)
            .BuildServiceProvider();

        var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
        {
            Services = services
        });

#if RELEASE
        slash.RegisterCommands<UnoSlashCommands>();
#elif DEBUG
        slash.RegisterCommands<UnoSlashCommands>(477149849632899072);
#endif

        discord.GuildDownloadCompleted += DiscordOnGuildDownloadCompleted;
        discord.ChannelDeleted += DiscordOnChannelDeleted;
        discord.ChannelCreated += DiscordOnChannelCreated;

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }

    private static Task DiscordOnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
    {
        _unoLibWrapper?.DeleteSettings(e.Channel.Id);
        return Task.CompletedTask;
    }

    private static Task DiscordOnChannelCreated(DiscordClient sender, ChannelCreateEventArgs e)
    {
        _unoLibWrapper?.SetSettings(e.Channel.Id);
        return Task.CompletedTask;
    }

    private static Task DiscordOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        _messageBuilder?.BuildEmojiDictionary(sender);
        foreach (var guild in e.Guilds)
        {
            foreach (ulong channel in guild.Value.Channels.Keys)
            {
                _unoLibWrapper?.SetSettings(channel);
            }
        }
        return Task.CompletedTask;
    }
}