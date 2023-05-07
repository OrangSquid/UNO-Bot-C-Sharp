using DSharpPlus;
using DSharpPlus.Entities;
using System.Collections;
using System.Resources;
using UNOLib;
using UNOLib.Cards;

namespace UNODiscordBot.Wrappers;

internal class GameSystemBuilderWrapper : GameSystemBuilder
{
    private static readonly Dictionary<CardColors, DiscordColor> DiscordColors = new();

    static GameSystemBuilderWrapper()
    {
        foreach (CardColors cardColor in Enum.GetValuesAsUnderlyingType<CardColors>())
        {
            DiscordColors.Add(cardColor, cardColor switch
            {
                CardColors.Red => new DiscordColor("#FF5555"),
                CardColors.Green => new DiscordColor("#55AA55"),
                CardColors.Yellow => new DiscordColor("#FFAA00"),
                CardColors.Blue => new DiscordColor("#5555FF"),
                CardColors.None => new DiscordColor("#000000"),
                _ => new DiscordColor("#FFFFFF")
            });
        }
    }

    public GameSystemBuilderWrapper CreatePlayers(List<DiscordUser> discordUsers)
    {
        for (var i = 0; i < discordUsers.Count; i++)
        {
            PlayersByOrder.Add(new DiscordPlayer(i, discordUsers[i]));
        }
        return this;
    }

    public override GameSystemWrapper Build()
    {
        return new GameSystemWrapper(PlayersByOrder, AllCardsDict, DrawStyle!, MustPlay, StackStyle!, JumpIn, UnoPenalty);
    }

    internal static void WrapCards(BaseDiscordClient client, ResourceSet rs)
    {
        AllCards.Clear();
        foreach (DictionaryEntry resource in rs)
        {
            var key = resource.Key.ToString();
            var value = Convert.ToUInt64(resource.Value);

            if (key == null || !AllCardsDict.TryGetValue(key, out var card)) continue;

            if (card is ColorCard colorCard)
            {
                var colorCardWrapped = new ColorCardWrapper(colorCard.Color, colorCard.Symbol)
                {
                    Emoji = DiscordEmoji.FromGuildEmote(client, value),
                    Choice = new DiscordAutoCompleteChoice(key, key),
                    DiscordColor = DiscordColors[colorCard.Color]
                };
                AllCardsDict[key] = colorCardWrapped;
                int i = colorCardWrapped.Symbol == ColorCardSymbols.Zero ? NumberOfZeros : NumberOfEachColorCard;
                for (; i > 0; i--)
                {
                    AllCards.Add(colorCardWrapped);
                }
            }
            else if (card is WildCard wildCard)
            {
                var wildCardWrapped = new WildCardWrapper(wildCard.Symbol)
                {
                    Emoji = DiscordEmoji.FromGuildEmote(client, value),
                    Choice = new DiscordAutoCompleteChoice(key, key),
                    DiscordColor = DiscordColors[wildCard.Color]
                };
                AllCardsDict[key] = wildCardWrapped;
                wildCardWrapped.WrapUnderlyingColoredWildCard(DiscordColors);
                for (var i = 0; i < NumberOfEachWildCard; i++)
                {
                    AllCards.Add(wildCardWrapped);
                }
            }
        }
    }
}