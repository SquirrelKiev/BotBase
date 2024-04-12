using Microsoft.EntityFrameworkCore;

namespace BotBase.Database;

public abstract class BotDbContextBase : DbContext
{
    protected string connectionString;

    protected BotDbContextBase(string connectionString)
    {
        this.connectionString = connectionString;
    }
}
