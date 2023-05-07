using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UNODiscordBot.Exceptions;
using UNODiscordBot.Wrappers;

namespace UNODiscordBot;

public class CardAutocompleteProvider : IAutocompleteProvider
{
    private static readonly DiscordAutoCompleteChoice[] NoGames;

    static CardAutocompleteProvider()
    {
        NoGames = new[]
        {
            new DiscordAutoCompleteChoice("No games available", " ")
        };
    }

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        var ulw = ctx.Services.GetService<UnoLibWrapper>();
        try
        {
            var player = ulw!.GetPlayer(ctx.Channel.Id, ctx.User);
            var cardChoices = new List<DiscordAutoCompleteChoice>(player.NumCards);
            cardChoices.AddRange(
                from card in player
                let optionString = ctx.Interaction.Data.Options.ElementAt(0).Value.ToString()
                where card.ToString().ToLower().Contains(optionString!.ToLower())
                select ((ICardWrapper)card).Choice
            );
            return cardChoices;
        }
        catch (GameDoesNotExistException)
        {
            return NoGames;
        }
    }
}