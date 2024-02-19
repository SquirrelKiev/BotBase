using System.Diagnostics.CodeAnalysis;
using BotBase.Database;

namespace BotBase.Examples.TodoList.Database.Models;

public class TodoEntry : DbModel
{
    [SetsRequiredMembers]
    public TodoEntry(ulong userId, string contents)
    {
        UserId = userId;
        Contents = contents;
    }

    public required ulong UserId { get; set; }
    public required string Contents { get; set; }
}