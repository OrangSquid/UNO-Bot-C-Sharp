using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;

namespace UNODiscordBot;

public class UNOLibWrapper
{
    private readonly Dictionary<ulong, List<DiscordUser>> _guild_lobbies;
    private readonly Dictionary<ulong, GameStruct> _guild_games;

    public UNOLibWrapper()
    {
        _guild_lobbies = new();
        _guild_games = new();
    }

    public void CreateGame(ulong guildId, DiscordUser user)
    {
        if (_guild_lobbies.ContainsKey(guildId) || _guild_games.ContainsKey(guildId))
        {
            throw new GameAlreadyExistsException();
        }
        List<DiscordUser> user_list = new()
        {
            user
        };
        _guild_lobbies.Add(guildId, user_list);
    }

    public void JoinGame(ulong guildId, DiscordUser user)
    {
        if (!_guild_lobbies.TryGetValue(guildId, out List<DiscordUser>? lobby))
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
        if (_guild_games.ContainsKey(guildId))
        {
            throw new GameAlreadyStartedException();
        }
        if (!_guild_lobbies.TryGetValue(guildId, out List<DiscordUser>? lobby))
        {
            throw new GameDoesNotExistException();
        }
        int nPlayers = lobby.Count;
        GameSystemFactory gsf = new(nPlayers)
        {
            DrawUntilPlayableCard = false
        };
        GameSystem gs = gsf.Build();
        _guild_games.Add(guildId, new GameStruct()
        {
            Gs = gs,
            Players = lobby
        });
        _guild_lobbies.Remove(guildId);
        return gs.State;
    }

    public IPlayer CheckCards(ulong guildId, DiscordUser player)
    {
        if (!_guild_games.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        return GetPlayer(game, player);
    }

    public GameState PlayCard(ulong guildId, DiscordUser player, string card)
    {
        if (!_guild_games.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.CardPlay(GetPlayerId(game, player), card);
        return game.Gs.State;
    }

    public GameState DrawCard(ulong guildId, DiscordUser player)
    {
        if (!_guild_games.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.DrawCard(GetPlayerId(game, player));
        return game.Gs.State;
    }

    public GameState ChangeColor(ulong guildId, DiscordUser player, string color)
    {
        if (!_guild_games.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.ChangeOnTableColor(GetPlayerId(game, player), color);
        return game.Gs.State;
    }

    public DiscordUser GetUser(ulong guildId, int playerId)
    {
        if (!_guild_games.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        return game.Players[playerId];
    }

    private static int GetPlayerId(GameStruct game, DiscordUser player)
    {
        int playerId = game.Players.IndexOf(player);
        if (playerId == -1)
        {
            throw new PlayerDoesNotExistException();
        }
        return playerId;
    }

    private static IPlayer GetPlayer(GameStruct game, DiscordUser player)
    {
        return game.Gs.GetPlayer(GetPlayerId(game, player));
    }
}
