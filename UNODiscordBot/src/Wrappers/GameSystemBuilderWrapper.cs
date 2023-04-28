using DSharpPlus.Entities;
using UNOLib;

namespace UNODiscordBot.Wrappers;

internal class GameSystemBuilderWrapper : GameSystemBuilder
{
    public GameSystemBuilderWrapper() : base() { }

    public void CreatePlayers(List<DiscordUser> discordUsers)
    {
        for (var i = 0; i < discordUsers.Count; i++)
        {
            PlayersByOrder.Add(new DiscordPlayer(i, discordUsers[i]));
        }
    }
}