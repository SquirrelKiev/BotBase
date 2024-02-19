using BotBase.Modules.ConfigCommand;

namespace BotBase.Examples.BarebonesWithExtras.Modules.ConfigCommand.Pages;

public abstract class ConfigPage : ConfigPageBase<ConfigPage.Page>
{
    public enum Page
    {
        Help,
        Prefix
    }
}
