using BotBase.Database;
using BotBase.Examples.TodoList.Database;
using BotBase.Modules.Help;
using Discord.Commands;

namespace BotBase.Examples.TodoList.Modules.Help;

public class HelpPrefixModule : PrefixModule
{
    private readonly DbService dbService;
    private readonly HelpService helpService;
    private readonly BotConfigBase botConfig;

    public HelpPrefixModule(DbService dbService, HelpService helpService, BotConfigBase botConfig)
    {
        this.dbService = dbService;
        this.helpService = helpService;
        this.botConfig = botConfig;
    }

    [Command("help")]
    [ParentModulePrefix(typeof(HelpModule))]
    public async Task HelpCommand()
    {
        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id, botConfig.DefaultPrefix) : null;
        var contents = helpService.GetMessageContents(prefix);

        await ReplyAsync(contents);
    }
}