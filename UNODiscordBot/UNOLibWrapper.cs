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
        if (_guild_lobbies.ContainsKey(guildId))
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

    public void StartGame(ulong guildId)
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
        Settings settings = new()
        {
            DrawUntilPlayableCard = false
        };
        GameSystem gs = new(nPlayers, settings);
        _guild_games.Add(guildId, new GameStruct(gs, lobby));
        _guild_lobbies.Remove(guildId);
    }

    public IPlayer CheckCards(ulong guildId, DiscordUser player)
    {
        if (!_guild_games.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        return game.GetPlayer(player);
    }
}
