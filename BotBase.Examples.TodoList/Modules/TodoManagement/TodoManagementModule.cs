using Discord.Interactions;

namespace BotBase.Examples.TodoList.Modules.TodoManagement;

[Group("todo", "Commands related to your todo list.")]
public class TodoManagementModule : BotModule
{
    private readonly TodoManagementService todoManagementService;

    public TodoManagementModule(TodoManagementService todoManagementService)
    {
        this.todoManagementService = todoManagementService;
    }

    [SlashCommand("list", "Pulls up your todo list.")]
    public async Task ListSlash()
    {
        await DeferAsync();

        await FollowupAsync(await todoManagementService.GetContents_List(Context.User.Id));
    }

    [SlashCommand("add", "Adds an entry to your todo list.")]
    public async Task AddSlash([Summary(description:"The entry to add. Will be appended to the bottom of the list.")] string entry)
    {
        await DeferAsync();

        await FollowupAsync(await todoManagementService.GetContents_Add(Context.User.Id, entry));
    }

    [SlashCommand("remove", "Removes an entry from your todo list.")]
    public async Task RemoveSlash([Summary(description: "The ID of the entry to remove. Can be seen next to the entry in the list command.")] int id)
    {
        await DeferAsync();

        await FollowupAsync(await todoManagementService.GetContents_Remove(Context.User.Id, id));
    }
}