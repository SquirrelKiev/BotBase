using Discord;
using Discord.Interactions;

namespace BotBase.Modules.About;

public abstract class AboutModuleImpl : BotModule
{
    private readonly OverrideTrackerService overrideTrackerService;
    private readonly AboutService aboutService;

    protected AboutModuleImpl(AboutService aboutService, OverrideTrackerService overrideTrackerService)
    {
        this.aboutService = aboutService;
        this.overrideTrackerService = overrideTrackerService;
    }

    [ComponentInteraction(BaseModulePrefixes.ABOUT_OVERRIDE_TOGGLE)]
    public async Task OverrideToggleButton()
    {
        await DeferAsync();

        if (await overrideTrackerService.TryToggleOverride(Context.User.Id))
        {
            var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id);

            await ModifyOriginalResponseAsync(contents);
        }
        else
        {
            await RespondAsync(new MessageContents("No permission.", embed: null, null), true);
        }
    }

    [CommandContextType(InteractionContextType.Guild, InteractionContextType.BotDm)]
    public virtual async Task AboutSlash()
    {
        await DeferAsync();

        var contents = await aboutService.GetMessageContents(await AboutService.GetPlaceholders(Context.Client), Context.User.Id);

        await FollowupAsync(contents);
    }
}