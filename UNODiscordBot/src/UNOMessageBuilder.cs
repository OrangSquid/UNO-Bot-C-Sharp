using DSharpPlus.Entities;
using System.Collections;
using System.Resources;
using System.Text;
using DSharpPlus;
using UNOLib.Player;

namespace UNODiscordBot;

public class UnoMessageBuilder
{
    private Dictionary<string, DiscordEmoji>? _emojisDictionary;

    internal void BuildEmojiDictionary(BaseDiscordClient client)
    {
        if (_emojisDictionary != null) return;

        _emojisDictionary = new Dictionary<string, DiscordEmoji>();

        var rm = new ResourceManager("UNODiscordBot.src.UNODiscordEmojis", typeof(Program).Assembly);
        var rs = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

        if (rs == null) return;

        foreach (DictionaryEntry resource in rs)
        {
            var key = resource.Key.ToString();
            var value = Convert.ToUInt64(resource.Value);
            if (key != null)
            {
                _emojisDictionary.Add(key, DiscordEmoji.FromGuildEmote(client, value));
            }
        }

        rm.ReleaseAllResources();
        rs.Dispose();
    }

    internal string PlayerHandToBackEmoji(IPlayer player)
    {
        if (_emojisDictionary == null) throw new InvalidOperationException();

        var emoji = _emojisDictionary["BackCard"];
        var message = new StringBuilder(player.NumCards);
        for (var i = 0; i < player.NumCards; i++)
        {
            message.Append(emoji);
        }
        return message.ToString();
    }

    internal string PlayerHandToFrontEmoji(IPlayer player)
    {
        if (_emojisDictionary == null) throw new InvalidOperationException();

        var message = new StringBuilder(player.NumCards);
        foreach (var card in player)
        {
            message.Append(_emojisDictionary[card.ToString()]);
        }
        return message.ToString();
    }
}
