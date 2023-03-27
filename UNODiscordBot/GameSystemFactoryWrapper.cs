﻿using DSharpPlus.Entities;
using UNOLib;

namespace UNODiscordBot;

internal class GameSystemFactoryWrapper : GameSystemFactory
{
    public GameSystemFactoryWrapper(int nPlayers) : base(nPlayers) { }

    public void CreatePlayers(List<DiscordUser> discordUsers)
    {
        if (discordUsers.Count != PlayersByOrder.Count)
        {
            throw new ArgumentException("discordUsers should have the same number of elements as nPlayers");
        }

        for (var i = 0; i < discordUsers.Count; i++)
        {
            PlayersByOrder.Add(new DiscordPlayer(i, discordUsers[i]));
        }
    }
}