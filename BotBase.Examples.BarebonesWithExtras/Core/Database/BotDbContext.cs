using BotBase.Database;

namespace BotBase.Examples.BarebonesWithExtras.Database;

public abstract class BotDbContext : BotDbContextBase
{
    protected BotDbContext(string connectionString) : base(connectionString)
    {
    }
}