using Discord;

namespace BotBase.Modules.ConfigCommand;

/// <typeparam name="T">T should be an enum containing every page. 0 should ALWAYS be "Help", and will be used to show info about the config options.</typeparam>
public abstract class ConfigPageBase<T> : BotModule where T : Enum
{
    public abstract T Id { get; }
    // could probably be replaced with humanizer stuff and use the enum but i like it mostly being in all one place
    public abstract string Label { get; }
    public abstract string Description { get; }
    public abstract bool EnabledInDMs { get; }

    public bool ShouldShow(bool isDm)
    {
        return !isDm || EnabledInDMs;
    }

    public abstract Task<MessageContents> GetMessageContents(ConfigCommandServiceBase<T>.State state);

    public SelectMenuBuilder GetPageSelectDropdown(Dictionary<T, ConfigPageBase<T>> pages, T id, bool isDm)
    {
        var dropdown = new SelectMenuBuilder()
            .WithCustomId(BaseModulePrefixes.CONFIG_PAGE_SELECT_PAGE);

        foreach (var page in pages.Values.Where(page => page.ShouldShow(isDm)))
        {
            dropdown
                .AddOption(new SelectMenuOptionBuilder()
                    .WithLabel(page.Label)
                    .WithValue(StateSerializer.SerializeObject(page.Id))
                    .WithDefault(page.Id.Equals(id))
                    .WithDescription(page.Description));
        }

        return dropdown;
    }
}