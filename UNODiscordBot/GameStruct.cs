using DSharpPlus.Entities;
using UNOLib;

namespace UNODiscordBot;

internal struct GameStruct
{
    public IGameSystem Gs { get; init; }
    public List<DiscordUser> Players { get; init; }
}