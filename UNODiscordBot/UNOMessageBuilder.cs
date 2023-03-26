using System.Collections;
using System.Resources;

namespace UNODiscordBot;

internal class UNOMessageBuilder
{

    internal static readonly Dictionary<string, ulong> emojiIds;
    static UNOMessageBuilder()
    {
        emojiIds = new();
        
        ResourceManager rm = new ResourceManager("UNODiscordBot.UNODiscordEmojis", typeof(Program).Assembly);
        ResourceSet rs = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

        foreach (DictionaryEntry resource in rs)
        {
            string key = resource.Key.ToString();
            ulong value = Convert.ToUInt64(resource.Value);
            emojiIds.Add(key, value);
        }

    }
}
