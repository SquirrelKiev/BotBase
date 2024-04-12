using BotBase.Database;
using BotBase.Examples.TodoList.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BotBase.Examples.TodoList.Database;

public abstract class BotDbContext : BotDbContextPrefixBase
{
    public DbSet<TodoEntry> TodoEntries { get; set; }

    protected BotDbContext(string connectionString) : base(connectionString)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoEntry>()
            .HasIndex(x => x.UserId);
    }
}