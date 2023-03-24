using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Cards;
using UNOLib.Player;

namespace UNODiscordBot;

public class UnoLibWrapper
{
    private readonly Dictionary<ulong, List<DiscordUser>> _guildLobbies;
    private readonly Dictionary<ulong, GameStruct> _guildGames;

    public UnoLibWrapper()
    {
        _guildLobbies = new();
        _guildGames = new();
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
        if (!_guildLobbies.TryGetValue(guildId, out List<DiscordUser>? lobby))
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
        if (!_guildLobbies.TryGetValue(guildId, out List<DiscordUser>? lobby))
        {
            throw new GameDoesNotExistException();
        }
        int nPlayers = lobby.Count;
        GameSystemFactory gsf = new(nPlayers)
        {
            DrawUntilPlayableCard = false,
            StackPlusTwo = true,
            MustPlay = false,
            JumpIn = true
        };
        var gs = gsf.Build();
        _guildGames.Add(guildId, new GameStruct()
        {
            Gs = gs,
            Players = lobby
        });
        _guildLobbies.Remove(guildId);
        return gs.State;
    }

    public IPlayer CheckCards(ulong guildId, DiscordUser player)
    {
        if (!_guildGames.TryGetValue(guildId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        return GetPlayer(game, player);
    }

    public GameState PlayCard(ulong guildId, DiscordUser player, string card)
    {
        if (!_guildGames.TryGetValue(guildId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.CardPlay(GetPlayerId(game, player), card);
        return game.Gs.State;
    }

    public GameState DrawCard(ulong guildId, DiscordUser player)
    {
        if (!_guildGames.TryGetValue(guildId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.DrawCard(GetPlayerId(game, player));
        return game.Gs.State;
    }

    public GameState ChangeColor(ulong guildId, DiscordUser player, string color)
    {
        if (!_guildGames.TryGetValue(guildId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.ChangeOnTableColor(GetPlayerId(game, player), color);
        return game.Gs.State;
    }

    public GameState Skip(ulong guildId, DiscordUser player)
    {
        if (!_guildGames.TryGetValue(guildId, out GameStruct game))
        {
            throw new GameDoesNotExistException();
        }
        game.Gs.Skip(GetPlayerId(game, player));
        return game.Gs.State;
    }

    public DiscordUser GetUser(ulong guildId, int playerId)
    {
        if (!_guildGames.TryGetValue(guildId, out var game))
        {
            throw new GameDoesNotExistException();
        }
        return game.Players[playerId];
    }

    public string CardURL(ICard card)
    {
        if (card is WildCard wc)
        {
            return "wild%20" + wc.Symbol.ToString().ToLower() + ".png";
        }
        else if (card is ColorCard cc)
        {
            string message = cc.Color.ToString().ToLower() + "%20";

            if (cc.Symbol.Equals(ColorCardSymbols.Reverse) || cc.Symbol.Equals(ColorCardSymbols.PlusTwo) || cc.Symbol.Equals(ColorCardSymbols.Skip))
                message += cc.Symbol.ToString().ToLower();
            else
                message += Enum.Format(typeof(ColorCardSymbols), cc.Symbol, "d");
            message += ".png";

            return message;
        }

        return "";
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