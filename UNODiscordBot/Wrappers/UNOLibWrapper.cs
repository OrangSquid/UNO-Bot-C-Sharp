﻿using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Player;

namespace UNODiscordBot.Wrappers;

public class UnoLibWrapper
{
    private readonly Dictionary<ulong, List<DiscordUser>> _guildLobbies;
    private readonly Dictionary<ulong, GameStruct> _guildGames;

    public UnoLibWrapper()
    {
        _guildLobbies = new Dictionary<ulong, List<DiscordUser>>();
        _guildGames = new Dictionary<ulong, GameStruct>();
    }

    public void CreateGame(ulong guildId, DiscordUser user)
    {
        if (_guildLobbies.ContainsKey(guildId) || _guildGames.ContainsKey(guildId))
        {
            throw new GameAlreadyExistsException();
        }
        List<DiscordUser> userList = new()
        {
            user
        };
        _guildLobbies.Add(guildId, userList);
    }

    public void JoinGame(ulong guildId, DiscordUser user)
    {
        if (!_guildLobbies.TryGetValue(guildId, out var lobby))
        {
            throw new GameDoesNotExistException();
        }
        if (lobby.IndexOf(user) != -1)
        {
            throw new AlreadyPartOfTheGameException();
        }
        lobby.Insert(lobby.Count, user);
    }

    public GameState StartGame(ulong guildId)
    {
        if (_guildGames.ContainsKey(guildId))
        {
            throw new GameAlreadyStartedException();
        }
        if (!_guildLobbies.TryGetValue(guildId, out var lobby))
        {
            throw new GameDoesNotExistException();
        }
        int nPlayers = lobby.Count;
        GameSystemFactoryWrapper gsf = new(nPlayers)
        {
            DrawUntilPlayableCard = false,
            StackPlusTwo = true,
            MustPlay = false,
            JumpIn = true,
            UnoPenalty = 2
        };
        gsf.CreatePlayers(lobby);
        var gs = gsf.Build();
        _guildGames.Add(guildId, new GameStruct()
        {
            Gs = gs,
            Players = lobby
        });
        _guildLobbies.Remove(guildId);
        return gs.State;
    }

    public IPlayer GetPlayer(ulong guildId, DiscordUser player)
    {
        var game = GetGame(guildId);
        return GetPlayer(game, player);
    }

    public GameState PlayCard(ulong guildId, DiscordUser player, string card)
    {
        var game = GetGame(guildId);
        game.Gs.CardPlay(GetPlayerId(game, player), card);

        if (game.Gs.State.GameFinished)
            _guildGames.Remove(guildId);

        return game.Gs.State;
    }

    public GameState DrawCard(ulong guildId, DiscordUser player)
    {
        var game = GetGame(guildId);
        game.Gs.DrawCard(GetPlayerId(game, player));
        return game.Gs.State;
    }

    public GameState ChangeColor(ulong guildId, DiscordUser player, string color)
    {
        var game = GetGame(guildId);
        game.Gs.ChangeOnTableColor(GetPlayerId(game, player), color);
        return game.Gs.State;
    }

    public GameState Skip(ulong guildId, DiscordUser player)
    {
        var game = GetGame(guildId);
        game.Gs.Skip(GetPlayerId(game, player));
        return game.Gs.State;
    }

    private int GetPlayerId(GameStruct game, DiscordUser player)
    {
        int playerId = game.Players.IndexOf(player);
        if (playerId == -1)
        {
            throw new PlayerDoesNotExistException();
        }
        return playerId;
    }

    private IPlayer GetPlayer(GameStruct game, DiscordUser player)
    {
        return game.Gs.GetPlayer(GetPlayerId(game, player));
    }

    internal List<DiscordUser> GetDiscordUsers(ulong guildId)
    {
        var game = GetGame(guildId);
        return game.Players;
    }

    private GameStruct GetGame(ulong guildId)
    {
        if (!_guildGames.TryGetValue(guildId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        return game;
    }
}