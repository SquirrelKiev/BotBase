namespace BotBase.Database;

public static class PrefixExtensions
{
    public static async Task SetPrefix<T>(this DbServiceBase<T> dbService, ulong guildId, string prefix) where T : BotDbContextPrefixBase
    {
        var dbContext = dbService.GetDbContext();

        await dbContext.GuildPrefixPreferences.UpsertAsync(x => x.GuildId == guildId, config => config.Prefix = prefix);

        await dbContext.SaveChangesAsync();
    }

    public static async Task<string> GetPrefix<T>(this DbServiceBase<T> dbService, ulong guildId, string defaultPrefix) where T : BotDbContextPrefixBase
    {
        var dbContext = dbService.GetDbContext();

        var guild = await dbContext.GuildPrefixPreferences.GetOrAddAsync(x => x.GuildId == guildId, () => new GuildPrefixPreference
        {
            GuildId = guildId,
            Prefix = defaultPrefix
        });
        
        await dbContext.SaveChangesAsync();

        return guild.Prefix;
    }
}
