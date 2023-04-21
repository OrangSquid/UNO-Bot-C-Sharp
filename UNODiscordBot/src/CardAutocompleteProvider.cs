using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UNODiscordBot.Exceptions;
using UNODiscordBot.Wrappers;

namespace UNODiscordBot;

public class CardAutocompleteProvider : IAutocompleteProvider
{
#pragma warning disable CS1998
    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
#pragma warning restore CS1998
    {
        var ulw = ctx.Services.GetService<UnoLibWrapper>();
        try
        {
            var player = ulw.GetPlayer(ctx.Channel.Id, ctx.User);
            var cardChoices = new List<DiscordAutoCompleteChoice>(player.NumCards);
            cardChoices.AddRange(player.Select(card => new DiscordAutoCompleteChoice(card.ToString(), card.ToString())));
            return cardChoices;
        }
        catch (GameDoesNotExistException)
        {
            return new[]
            {
                new DiscordAutoCompleteChoice("No games available", " ")
            };
        }
    }
}