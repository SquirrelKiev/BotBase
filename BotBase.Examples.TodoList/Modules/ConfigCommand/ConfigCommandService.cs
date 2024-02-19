using BotBase.Examples.TodoList.Modules.ConfigCommand.Pages;
using BotBase.Modules.ConfigCommand;

namespace BotBase.Examples.TodoList.Modules.ConfigCommand;

public class ConfigCommandService : ConfigCommandServiceBase<ConfigPage.Page>
{
    public ConfigCommandService(IServiceProvider services) : base(services)
    {
    }
}