namespace BotBase;

// officially the stupidest service to exist.
// 100% could be extension methods on ICacheProvider, but that felt wrong.
/// <remarks>None of the functions check or care if the user is allowed to have an override. Make sure to check with <see cref="BotConfigBase"/> before using these.</remarks>
public class OverrideTrackerService
{
    private readonly ICacheProvider cacheProvider;
    private readonly BotConfigBase botConfig;

    public OverrideTrackerService(ICacheProvider cacheProvider, BotConfigBase botConfig)
    {
        this.cacheProvider = cacheProvider;
        this.botConfig = botConfig;
    }

    public async ValueTask SetOverride(ulong id) => await cacheProvider.SetAsync(GetKeyName(id), true, new CacheValueSettings(TimeSpan.FromMinutes(15)));

    public async ValueTask ClearOverride(ulong id) => await cacheProvider.RemoveAsync(GetKeyName(id));

    public async ValueTask<bool> HasOverride(ulong id) => (await cacheProvider.GetAsync<bool>(GetKeyName(id))).IsSpecified;

    private static string GetKeyName(ulong id) => $"override-{id}";

    public async ValueTask<bool> TryToggleOverride(ulong userId)
    {
        if (!botConfig.ManagerUserIds.Contains(userId)) return false;

        if (!await HasOverride(userId))
        {
            await SetOverride(userId);
        }
        else
        {
            await ClearOverride(userId);
        }

        return true;

    }
}
