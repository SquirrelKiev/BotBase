using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BotBase.Database;

public abstract class BotDbContextBase : DbContext
{
    public DbSet<GuildPrefixPreference> GuildPrefixPreferences { get; set; }

    protected string connectionString;

    protected BotDbContextBase(string connectionString)
    {
        this.connectionString = connectionString;
    }
}
