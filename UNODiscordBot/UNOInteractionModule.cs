using Discord.Interactions;

namespace UNODiscordBot;

public class UNOInteractionModule : InteractionModuleBase
{
    [SlashCommand("new", "a")]
    public async Task NewGame()
    {
        await RespondAsync("lmao");
    }

    public override void OnModuleBuilding(InteractionService commandService, ModuleInfo module)
    {
        Console.WriteLine("i'm being built");
    }
}
