using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UNODiscordBot.Exceptions;
using UNOLib;

namespace UNODiscordBot;

public class CardAutocompleteProvider : IAutocompleteProvider
{
    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        var ulw = ctx.Services.GetService<UNOLibWrapper>();
        try
        {
            IPlayer player = ulw.CheckCards(ctx.Guild.Id, ctx.User);
            var cardChoices = new List<DiscordAutoCompleteChoice>(player.NumCards);
            int i = 0;
            foreach(ICard card in player)
            {
                cardChoices.Add(new DiscordAutoCompleteChoice(card.ToString(), "lmao"));
            }
            return cardChoices;
        }
        // TODO catch proper exceptions
        catch(Exception)
        {
            return new DiscordAutoCompleteChoice[] 
            {
                new DiscordAutoCompleteChoice("testing", "testing")
            };
        }
    }
}
