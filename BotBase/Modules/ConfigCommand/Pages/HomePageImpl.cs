using Discord;

namespace BotBase.Modules.ConfigCommand;

public class HomePageImpl<T> where T : Enum
{
    private readonly ConfigCommandServiceBase<T> configCommandService;
    private readonly ConfigPageBase<T> configPage;

    public HomePageImpl(ConfigCommandServiceBase<T> configCommandService, ConfigPageBase<T> configPage)
    {
        this.configCommandService = configCommandService;
        this.configPage = configPage;
    }

    public Task<MessageContents> GetMessageContents(ConfigCommandServiceBase<T>.State state)
    {
        var embed = new EmbedBuilder()
            .WithColor(CommandResult.Default);

        foreach (var page in configCommandService.ConfigPages.Values.Where(page => page.ShouldShow(configPage.IsDm())))
        {
            embed.AddField(page.Label, page.Description);
        }

        var components = new ComponentBuilder()
            .WithSelectMenu(configPage.GetPageSelectDropdown(configCommandService.ConfigPages, configPage.Id, configPage.IsDm()))
            .WithRedButton();

        return Task.FromResult(new MessageContents("", embed.Build(), components));
    }
}