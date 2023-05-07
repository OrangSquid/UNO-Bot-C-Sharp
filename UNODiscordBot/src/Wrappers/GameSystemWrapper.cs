using DSharpPlus.Entities;
using UNODiscordBot.Exceptions;
using UNOLib;
using UNOLib.Cards;
using UNOLib.DrawStyle;
using UNOLib.Player;
using UNOLib.StackStyles;

namespace UNODiscordBot.Wrappers;

internal class GameSystemWrapper : GameSystem
{

    public DiscordPlayer GetPlayer(DiscordUser user)
    {
        var player = PlayersByOrder.FirstOrDefault(p => (p as DiscordPlayer)!.User == user) as DiscordPlayer
                     ?? throw new PlayerDoesNotExistException();
        return player;
    }

    protected internal GameSystemWrapper(List<IPlayer> playersByOrder, Dictionary<string, ICard> allCardsDict, IDrawStyle drawStyle, bool mustPlay, IStackStyle stackStyle, bool jumpIn, int unoPenalty) : base(playersByOrder, allCardsDict, drawStyle, mustPlay, stackStyle, jumpIn, unoPenalty)
    {
    }
}