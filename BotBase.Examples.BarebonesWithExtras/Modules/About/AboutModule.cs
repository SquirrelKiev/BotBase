using BotBase.Modules.About;

namespace BotBase.Examples.BarebonesWithExtras.Modules.About;

public class AboutModule : AboutModuleImpl
{
    public AboutModule(AboutService aboutService, OverrideTrackerService overrideTrackerService) : base(aboutService, overrideTrackerService)
    {
    }
}