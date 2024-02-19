using BotBase.Examples.TodoList.Database;
using BotBase.Examples.TodoList.Database.Models;
using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace BotBase.Examples.TodoList.Modules.TodoManagement;

[Inject(ServiceLifetime.Singleton)]
public class TodoManagementService(DbService dbService)
{
    public async Task<MessageContents> GetContents_List(ulong userId)
    {
        await using var context = dbService.GetDbContext();

        List<EmbedFieldBuilder> fields = await context.TodoEntries.Where(x => x.UserId == userId)
            .Select(todoEntry => new EmbedFieldBuilder().WithName($"ID: {todoEntry.Id}")
                .WithValue(todoEntry.Contents))
            .ToListAsync();

        if (fields.Count > 0)
        {
            var embed = new EmbedBuilder()
                .WithFields(fields);

            return new MessageContents("", embed.Build(), null);
        }
        else
        {
            var embed = new EmbedBuilder()
                .WithDescription("No entries!");

            return new MessageContents("", embed.Build(), null);
        }
    }

    public async Task<MessageContents> GetContents_Add(ulong userId, string entry)
    {
        await using var context = dbService.GetDbContext();

        context.TodoEntries.Add(new TodoEntry(userId, entry));

        await context.SaveChangesAsync();

        return new MessageContents("Added successfully.", embed: null, null);
    }

    public async Task<MessageContents> GetContents_Remove(ulong userId, int id)
    {
        await using var context = dbService.GetDbContext();

        var entryToRemove = await context.TodoEntries.FirstOrDefaultAsync(x =>
            x.Id == id && x.UserId == userId
        );

        if (entryToRemove != null)
        {
            context.TodoEntries.Remove(entryToRemove);

            await context.SaveChangesAsync();

            return new MessageContents("Removed successfully.", embed: null, null);
        }
        else
        {
            return new MessageContents("Failed to remove: entry does not exist or it is not your entry.", embed: null, null);
        }
    }
}