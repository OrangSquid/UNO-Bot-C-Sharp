using DSharpPlus.Entities;
using UNOLib;

namespace UNODiscordBot;

internal struct GameStruct
{
    public GameSystem Gs { get; init; }
    public List<DiscordUser> Players { get; init; }
}
