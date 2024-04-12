using BotBase.Database;

namespace BotBase.Examples.BarebonesWithExtras.Database;

public abstract class BotDbContext : BotDbContextPrefixBase
{
    protected BotDbContext(string connectionString) : base(connectionString)
    {
    }
}