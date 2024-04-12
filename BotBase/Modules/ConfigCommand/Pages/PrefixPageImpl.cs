using BotBase.Database;
using Discord;
using Discord.Interactions;

namespace BotBase.Modules.ConfigCommand;

public class SetPrefixModal : IModal
{
    public string Title => "Set Prefix";

    [ModalTextInput(customId: BaseModulePrefixes.CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX, minLength: 1, maxLength: 10)]
    public string Prefix { get; set; } = "";
}

public class PrefixPageImpl<TPageEnum,TDatabaseContext> where TPageEnum : Enum where TDatabaseContext : BotDbContextPrefixBase
{
    private readonly ConfigCommandServiceBase<TPageEnum> configCommandService;
    private readonly DbServiceBase<TDatabaseContext> dbService;
    private readonly BotConfigBase botConfig;

    private readonly ConfigPageBase<TPageEnum> configPage;

    public PrefixPageImpl(ConfigCommandServiceBase<TPageEnum> configCommandService,
        DbServiceBase<TDatabaseContext> dbService, ConfigPageBase<TPageEnum> configPage, BotConfigBase botConfig)
    {
        this.configCommandService = configCommandService;
        this.dbService = dbService;
        this.botConfig = botConfig;
        this.configPage = configPage;
    }

    public async Task<MessageContents> GetMessageContents()
    {
        var prefix = await dbService.GetPrefix(configPage.Context.Guild.Id, botConfig.DefaultPrefix);

        var embed = new EmbedBuilder()
            .WithFields(new EmbedFieldBuilder()
                .WithName("Prefix")
                .WithValue($"`{prefix}`"))
            .WithColor(CommandResult.Default);

        var components = new ComponentBuilder()
            .WithSelectMenu(configPage.GetPageSelectDropdown(configCommandService.ConfigPages, configPage.Id, configPage.IsDm()))
            .WithButton("Change Prefix", BaseModulePrefixes.CONFIG_PREFIX_MODAL_BUTTON, ButtonStyle.Secondary)
            .WithRedButton();

        return new MessageContents(string.Empty, embed.Build(), components);
    }

    public async Task OnChangeButton()
    {
        var prefix = await dbService.GetPrefix(configPage.Context.Guild.Id, botConfig.DefaultPrefix);

        await configPage.Context.Interaction.RespondWithModalAsync<SetPrefixModal>(BaseModulePrefixes.CONFIG_PREFIX_MODAL, modifyModal:
            builder =>
            {
                builder.UpdateTextInput(BaseModulePrefixes.CONFIG_PREFIX_MODAL_PREFIX_TEXTBOX,
                    input => input.Value = prefix);
            });
    }

    public Task OnModal(SetPrefixModal modal)
    {
        return dbService.SetPrefix(configPage.Context.Guild.Id, modal.Prefix);
    }
}