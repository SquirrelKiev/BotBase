using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BotBase.Database;

public abstract class DbServiceBase<T> where T : DbContext
{
    protected readonly BotConfigBase botConfig;

    protected DbServiceBase(BotConfigBase botConfig)
    {
        this.botConfig = botConfig;
    }

    public virtual async Task Initialize(bool migrationEnabled)
    {
        Log.Debug("Database migration: {migrationStatus}", migrationEnabled);

        var context = GetDbContext();

        PreMigration(context);

        if (migrationEnabled)
        {
            await context.Database.MigrateAsync();
        }
    }

    public virtual async Task ResetDatabase()
    {
        await using var dbContext = GetDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public abstract T GetDbContext();
    public virtual void PreMigration(T context){}
}
