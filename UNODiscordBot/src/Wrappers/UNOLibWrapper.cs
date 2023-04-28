using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Player;

namespace UNODiscordBot.Wrappers;

public class UnoLibWrapper
{
    private readonly Dictionary<ulong, List<DiscordUser>> _channelLobbies;
    private readonly Dictionary<ulong, GameStruct> _channelGames;
    private readonly Dictionary<ulong, GameSettings> _channelSettings;

    public UnoLibWrapper()
    {
        _channelLobbies = new Dictionary<ulong, List<DiscordUser>>();
        _channelGames = new Dictionary<ulong, GameStruct>();
        _channelSettings = new Dictionary<ulong, GameSettings>();
    }

    /// <summary>
    /// Sets default settings for all channels the bot is in
    /// </summary>
    /// <param name="channelId"></param>
    public void SetSettings(ulong channelId)
    {
        _channelSettings.Add(channelId, 
            new GameSettings
            {
                DrawUntilPlayableCard = false,
                JumpIn = false,
                MustPlay = false,
                StackPlusTwo = true,
                UnoPenalty = 2
            });
    }

    public void DeleteSettings(ulong channelId)
    {
        _channelSettings.Remove(channelId);
    }

    public void SetDrawUntilPlayableCard(ulong channelId, bool value)
    {
        var channelSetting = _channelSettings[channelId];
        channelSetting.DrawUntilPlayableCard = value;
        _channelSettings[channelId] = channelSetting;
    }

    public void SetStackPlusTwo(ulong channelId, bool value)
    {
        var channelSetting = _channelSettings[channelId];
        channelSetting.StackPlusTwo = value;
        _channelSettings[channelId] = channelSetting;
    }

    public void SetMustPlay(ulong channelId, bool value)
    {
        var channelSetting = _channelSettings[channelId];
        channelSetting.MustPlay = value;
        _channelSettings[channelId] = channelSetting;
    }

    public void SetJumpIn(ulong channelId, bool value)
    {
        var channelSetting = _channelSettings[channelId];
        channelSetting.JumpIn = value;
        _channelSettings[channelId] = channelSetting;
    }

    public void SetUnoPenalty(ulong channelId, int value)
    {
        var channelSetting = _channelSettings[channelId];
        channelSetting.UnoPenalty = value;
        _channelSettings[channelId] = channelSetting;
    }

    public void CreateGame(ulong channelId, DiscordUser user)
    {
        if (_channelLobbies.ContainsKey(channelId) || _channelGames.ContainsKey(channelId))
        {
            throw new GameAlreadyExistsException();
        }
        List<DiscordUser> userList = new()
        {
            user
        };
        _channelLobbies.Add(channelId, userList);
    }

    public void JoinGame(ulong channelId, DiscordUser user)
    {
        if (!_channelLobbies.TryGetValue(channelId, out var lobby))
        {
            throw new GameDoesNotExistException();
        }
        if (lobby.IndexOf(user) != -1)
        {
            throw new AlreadyPartOfTheGameException();
        }
        if (lobby.Count >= 6)
        {
            throw new TooManyPlayersException();
        }
        lobby.Insert(lobby.Count, user);
    }

    public void LeaveGame(ulong channelId, DiscordUser user)
    {
        if (!_channelLobbies.TryGetValue(channelId, out var lobby))
        {
            throw new GameDoesNotExistException();
        }
        if (lobby.IndexOf(user) == -1)
        {
            throw new PlayerDoesNotExistException();
        }
        if (lobby.Count == 1)
        {
            _channelLobbies.Remove(channelId);
            throw new NotEnoughPlayersException();
        }
        lobby.Remove(user);
    }

    public GameState StartGame(ulong channelId, DiscordUser user)
    {
        if (_channelGames.ContainsKey(channelId))
        {
            throw new GameAlreadyStartedException();
        }
        if (!_channelLobbies.TryGetValue(channelId, out var lobby))
        {
            throw new GameDoesNotExistException();
        }

        if (_channelLobbies[channelId].IndexOf(user) == -1)
        {
            throw new PlayerDoesNotExistException();
        }
#if RELEASE
        if (lobby.Count < 2)
        {
            throw new NotEnoughPlayersException();
        }
#endif
        int nPlayers = lobby.Count;
        var gs = new GameSystemBuilderWrapper()
            .CreatePlayers(nPlayers)
            .WithDrawUntilPlayable(_channelSettings[channelId].DrawUntilPlayableCard)
            .WithStackPlusTwo(_channelSettings[channelId].StackPlusTwo)
            .WithMustPlay(_channelSettings[channelId].MustPlay)
            .WithJumpIn(_channelSettings[channelId].JumpIn)
            .WithUnoPenalty(_channelSettings[channelId].UnoPenalty)
            .Build();

        _channelGames.Add(channelId, new GameStruct()
        {
            Gs = gs,
            Players = lobby
        });
        _channelLobbies.Remove(channelId);
        return gs.State;
    }

    public IPlayer GetPlayer(ulong channelId, DiscordUser player)
    {
        var game = GetGame(channelId);
        return GetPlayer(game, player);
    }

    public GameState PlayCard(ulong channelId, DiscordUser player, string card)
    {
        var game = GetGame(channelId);
        game.Gs.CardPlay(GetPlayerId(game, player), card);

        if (game.Gs.State.GameFinished)
            EndGame(channelId);

        return game.Gs.State;
    }

    public GameState DrawCard(ulong channelId, DiscordUser player)
    {
        var game = GetGame(channelId);
        game.Gs.DrawCard(GetPlayerId(game, player));
        return game.Gs.State;
    }

    public GameState ChangeColor(ulong channelId, DiscordUser player, string color)
    {
        var game = GetGame(channelId);
        game.Gs.ChangeOnTableColor(GetPlayerId(game, player), color);
        return game.Gs.State;
    }

    public GameState Skip(ulong channelId, DiscordUser player)
    {
        var game = GetGame(channelId);
        game.Gs.Skip(GetPlayerId(game, player));
        return game.Gs.State;
    }

    public bool EndGame(ulong channelId)
    {
        return _channelGames.Remove(channelId);
    }

    private IPlayer GetPlayer(GameStruct game, DiscordUser player)
    {
        return game.Gs.GetPlayer(GetPlayerId(game, player));
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
    internal List<DiscordUser> GetDiscordUsers(ulong channelId)
    {
        var game = GetGame(channelId);
        return game.Players;
    }

    private GameStruct GetGame(ulong channelId)
    {
        if (!_channelGames.TryGetValue(channelId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        return game;
    }
}