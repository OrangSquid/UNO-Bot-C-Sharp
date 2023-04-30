using DSharpPlus.Entities;
using UNOLib.Cards;

namespace UNODiscordBot.Wrappers;

internal interface ICardWrapper : ICard
{
    public DiscordEmoji? Emoji { get; init; }
    public DiscordAutoCompleteChoice? Choice { get; init; }
    public DiscordColor DiscordColor { get; init; }
    public string Url { get; }
}