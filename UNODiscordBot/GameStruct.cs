using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;

namespace UNODiscordBot;

internal struct GameStruct
{
    private readonly GameSystem _gs;
    private readonly List<DiscordUser> _players;

    public GameStruct(GameSystem gs, List<DiscordUser> players)
    {
        this._gs = gs;
        this._players = players;
    }

    public IPlayer GetPlayer(DiscordUser player)
    {
        int playerId = _players.IndexOf(player);
        if (playerId == -1)
        {
            throw new PlayerDoesNotExistException();
        }
        return _gs.GetPlayer(playerId);
    }
}
