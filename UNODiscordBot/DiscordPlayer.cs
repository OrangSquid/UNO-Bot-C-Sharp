using DSharpPlus.Entities;
using UNOLib.Player;

namespace UNODiscordBot;

internal class DiscordPlayer : BasePlayer
{
    public DiscordUser User { get; }

    public DiscordPlayer(int id, DiscordUser user) : base(id)
    {
        User = user;
    }
}