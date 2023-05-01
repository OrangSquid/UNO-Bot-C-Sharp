using DSharpPlus.Entities;
using UNOLib.Cards;
using HttpUtility = System.Web.HttpUtility;

namespace UNODiscordBot.Wrappers;

internal class WildCardWrapper : WildCard, ICardWrapper
{
    public DiscordEmoji? Emoji { get; init; }
    public DiscordAutoCompleteChoice? Choice { get; init; }
    public required DiscordColor DiscordColor { get; init; }
public string Url { get; }

public WildCardWrapper(WildCardSymbols symbol) : base(symbol)
{
    Url = $"https://raw.githubusercontent.com/OrangSquid/UNO-Bot-C-Sharp/main/deck/{Uri.EscapeDataString(ToString())}.png";
}

private WildCardWrapper(CardColors color, WildCardSymbols symbol) : base(color, symbol)
{
    Url = $"https://raw.githubusercontent.com/OrangSquid/UNO-Bot-C-Sharp/main/deck/{Uri.EscapeDataString(ToString())}.png";
}

public void WrapUnderlyingColoredWildCard(Dictionary<CardColors, DiscordColor> discordColors)
{
    for (var i = 0; i < ColoredWildCards!.Count; i++)
    {
        ColoredWildCards[i] = new WildCardWrapper(ColoredWildCards[i].Color, ColoredWildCards[i].Symbol)
        {
            DiscordColor = discordColors[ColoredWildCards[i].Color]
        };
    }
}
}