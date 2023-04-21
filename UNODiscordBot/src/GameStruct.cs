using DSharpPlus.Entities;
using UNOLib;

namespace UNODiscordBot;

// TODO maybe it's useless??????
internal struct GameStruct
{
    public IGameSystem Gs { get; init; }
    public List<DiscordUser> Players { get; init; }
}