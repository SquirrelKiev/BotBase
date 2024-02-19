using Discord;
using Discord.Interactions;

namespace BotBase.Modules.ConfigCommand;

/// <typeparam name="T">T should be an enum containing every page. 0 should ALWAYS be "Help", and will be used to show info about the config options.</typeparam>
public abstract class ConfigCommandModuleBase<T> : BotModule where T : Enum
{
    protected readonly ConfigCommandServiceBase<T> configService;

    protected ConfigCommandModuleBase(ConfigCommandServiceBase<T> configService)
    {
        this.configService = configService;
    }

    public virtual async Task ConfigSlash()
    {
        await DeferAsync();

        await FollowupAsync(await configService.GetMessageContents(new(), Context));
    }

    [ComponentInteraction(BaseModulePrefixes.CONFIG_PAGE_SELECT_PAGE)]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    public async Task SelectInteraction(string id)
    {
        await DeferAsync();

        var page = StateSerializer.DeserializeObject<T>(id)!;

        await ModifyOriginalResponseAsync(await configService.GetMessageContents(new(page: page, data: ""), Context));
    }

    [ComponentInteraction(BaseModulePrefixes.CONFIG_PAGE_SELECT_PAGE_BUTTON + "*")]
    [RequireUserPermission(GuildPermission.ManageGuild, Group = BaseModulePrefixes.PERMISSION_GROUP)]
    [HasOverride(Group = BaseModulePrefixes.PERMISSION_GROUP)]
    public Task SelectInteractionButton(string id)
    {
        return SelectInteraction(id);
    }
}
