using DSharpPlus.Entities;
using UNOLib.Cards;
using HttpUtility = System.Web.HttpUtility;

namespace UNODiscordBot.Wrappers;

internal class ColorCardWrapper : ColorCard, ICardWrapper
{
    public DiscordEmoji? Emoji { get; init; }
    public DiscordAutoCompleteChoice? Choice { get; init; }
    public required DiscordColor DiscordColor { get; init; }
public string Url { get; }

public ColorCardWrapper(CardColors colors, ColorCardSymbols symbol) : base(colors, symbol)
{
    Url = $"https://raw.githubusercontent.com/OrangSquid/UNO-Bot-C-Sharp/main/deck/{Uri.EscapeDataString(ToString())}.png";
    Console.WriteLine(Url);
}
}