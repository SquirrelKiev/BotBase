using Microsoft.EntityFrameworkCore;

namespace BotBase.Database;

public abstract class BotDbContextPrefixBase : BotDbContextBase
{
    public DbSet<GuildPrefixPreference> GuildPrefixPreferences { get; set; }

    protected BotDbContextPrefixBase(string connectionString) : base(connectionString)
    {
    }
}