using DSharpPlus.Entities;
using System.Resources;
using System.Text;
using DSharpPlus;
using UNODiscordBot.Wrappers;
using UNOLib.Player;

namespace UNODiscordBot;

public static class UnoMessageBuilder
{
    private static DiscordEmoji? _backCardEmoji;

    internal static void BuildEmojiDictionary(BaseDiscordClient client)
    {
        var rm = new ResourceManager("UNODiscordBot.src.UNODiscordEmojis", typeof(Program).Assembly);
        var rs = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

        if (rs == null) return;

        _backCardEmoji = DiscordEmoji.FromGuildEmote(client, Convert.ToUInt64(rs.GetObject("BackCard")));

        GameSystemBuilderWrapper.WrapCards(client, rs);

        rm.ReleaseAllResources();
        rs.Dispose();
    }

    internal static string PlayerHandToBackEmoji(IPlayer player)
    {
        var message = new StringBuilder(player.NumCards);
        for (var i = 0; i < player.NumCards; i++)
        {
            message.Append(_backCardEmoji);
        }
        return message.ToString();
    }

    internal static string PlayerHandToFrontEmoji(IPlayer player)
    {
        var message = new StringBuilder(player.NumCards);
        foreach (var card in player)
        {
            message.Append(((ICardWrapper)card).Emoji);
        }
        return message.ToString();
    }
}
