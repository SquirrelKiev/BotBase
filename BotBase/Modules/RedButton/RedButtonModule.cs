using BotBase.Modules;
using BotBase.Modules.RedButton;
using Discord;
using Discord.Interactions;

namespace BotBase.Modules.RedButton
{

    public class RedButtonModule : BotModule
    {
        [ComponentInteraction(BaseModulePrefixes.RED_BUTTON)]
        public async Task OnButton()
        {
            await DeferAsync();
            await Context.Interaction.DeleteOriginalResponseAsync();
        }
    }
}

namespace BotBase
{
    public static class RedButtonExtensions
    {
        public static ComponentBuilder WithRedButton(this ComponentBuilder componentBuilder, string label = "X", int row = 0)
        {
            componentBuilder.WithButton(label, BaseModulePrefixes.RED_BUTTON, ButtonStyle.Danger, row: row);

            return componentBuilder;
        }
    }
}
