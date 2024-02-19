﻿using BotBase.Database;
using BotBase.Examples.BarebonesWithExtras.Database;
using BotBase.Modules.Help;
using Discord.Interactions;

namespace BotBase.Examples.BarebonesWithExtras.Modules.Help;

public class HelpModule : BotModule
{
    private readonly DbService dbService;
    private readonly HelpService helpService;
    private readonly BotConfigBase botConfig;

    public HelpModule(DbService dbService, HelpService helpService, BotConfigBase botConfig)
    {
        this.dbService = dbService;
        this.helpService = helpService;
        this.botConfig = botConfig;
    }

    [SlashCommand("help", "Help! What are all the commands?")]
    [HelpPageDescription("Pulls up this page!")]
    [EnabledInDm(true)]
    public async Task HelpSlash()
    {
        await DeferAsync();

        var prefix = Context.Guild != null ? await dbService.GetPrefix(Context.Guild.Id, botConfig.DefaultPrefix) : null;
        var contents = helpService.GetMessageContents(prefix);

        await FollowupAsync(contents);
    }
}
