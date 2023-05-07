using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Player;

namespace UNODiscordBot.Wrappers;

public class UnoLibWrapper
{
    private readonly Dictionary<ulong, List<DiscordUser>> _channelLobbies;
    private readonly Dictionary<ulong, GameSystemWrapper> _channelGames;
    private readonly Dictionary<ulong, GameSettings> _channelSettings;

    public UnoLibWrapper()
    {
        _channelLobbies = new Dictionary<ulong, List<DiscordUser>>();
        _channelGames = new Dictionary<ulong, GameSystemWrapper>();
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
        var gs = new GameSystemBuilderWrapper()
            .CreatePlayers(lobby)
            .WithDrawUntilPlayable(_channelSettings[channelId].DrawUntilPlayableCard)
            .WithStackPlusTwo(_channelSettings[channelId].StackPlusTwo)
            .WithMustPlay(_channelSettings[channelId].MustPlay)
            .WithJumpIn(_channelSettings[channelId].JumpIn)
            .WithUnoPenalty(_channelSettings[channelId].UnoPenalty)
            .Build();

        _channelGames.Add(channelId, (gs as GameSystemWrapper)!);
        _channelLobbies.Remove(channelId);
        return gs.State;
    }

    public IPlayer GetPlayer(ulong channelId, DiscordUser player) => GetGame(channelId).GetPlayer(player);

    public GameState PlayCard(ulong channelId, DiscordUser player, string card)
    {
        var game = GetGame(channelId);
        game.CardPlay(GetPlayerId(game, player), card);

        if (game.State.GameFinished)
            EndGame(channelId, player);

        return game.State;
    }

    public GameState DrawCard(ulong channelId, DiscordUser player)
    {
        var game = GetGame(channelId);
        game.DrawCard(GetPlayerId(game, player));
        return game.State;
    }

    public GameState ChangeColor(ulong channelId, DiscordUser player, string color)
    {
        var game = GetGame(channelId);
        game.ChangeOnTableColor(GetPlayerId(game, player), color);
        return game.State;
    }

    public GameState Skip(ulong channelId, DiscordUser player)
    {
        var game = GetGame(channelId);
        game.Skip(GetPlayerId(game, player));
        return game.State;
    }

    public void EndGame(ulong channelId, DiscordUser player)
    {
        _channelLobbies.TryGetValue(channelId, out var lobby);
        _channelGames.TryGetValue(channelId, out var game);
        if (lobby == null && game == null)
        {
            throw new GameDoesNotExistException();
        }
        if (lobby != null && lobby.IndexOf(player) == -1)
        {
            throw new PlayerDoesNotExistException();
        }
        game?.GetPlayer(player);
        _channelGames.Remove(channelId);
        _channelLobbies.Remove(channelId);
    }

    private int GetPlayerId(GameSystemWrapper game, DiscordUser player)
    {
        return game.GetPlayer(player).Id;
    }

    internal GameSystemWrapper GetGame(ulong channelId)
    {
        if (!_channelGames.TryGetValue(channelId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        return game;
    }
}