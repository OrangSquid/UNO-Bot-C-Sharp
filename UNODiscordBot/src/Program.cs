using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
#if RELEASE
using Microsoft.Extensions.Logging;
#endif
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
#if RELEASE
            , MinimumLogLevel = LogLevel.Critical
#endif
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
        discord.GuildCreated += DiscordOnGuildCreated;
        discord.GuildDeleted += DiscordOnGuildDeleted;

        await discord.ConnectAsync();
        await Task.Delay(-1);
    }

    private static async Task DiscordOnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
    {
        _unoLibWrapper?.DeleteSettings(e.Channel.Id);
    }

    private static async Task DiscordOnChannelCreated(DiscordClient sender, ChannelCreateEventArgs e)
    {
        _unoLibWrapper?.SetSettings(e.Channel.Id);
    }

    private static async Task DiscordOnGuildCreated(DiscordClient sender, GuildCreateEventArgs e)
    {
        foreach (var channel in e.Guild.Channels.Keys)
        {
            _unoLibWrapper?.SetSettings(channel);
        }
    }

    private static async Task DiscordOnGuildDeleted(DiscordClient sender, GuildDeleteEventArgs e)
    {
        foreach (var channel in e.Guild.Channels.Keys)
        {
            _unoLibWrapper?.DeleteSettings(channel);
        }
    }

    private static async Task DiscordOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        _messageBuilder?.BuildEmojiDictionary(sender);
        foreach (var guild in e.Guilds)
        {
            foreach (ulong channel in guild.Value.Channels.Keys)
            {
                _unoLibWrapper?.SetSettings(channel);
            }
        }
    }
}