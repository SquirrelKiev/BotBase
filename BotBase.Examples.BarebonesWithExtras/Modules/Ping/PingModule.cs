using Discord.Interactions;
using Discord.WebSocket;

namespace BotBase.Examples.BarebonesWithExtras.Modules.Ping;

public class PingModule : BotModule
{
    private readonly PingService pingService;

    public PingModule(PingService pingService)
    {
        this.pingService = pingService;
    }

    [SlashCommand("ping", "Pong!")]
    public async Task PingSlash()
    {
        await DeferAsync();

        await FollowupAsync(pingService.GetMessageContents((DiscordSocketClient)Context.Client));
    }
}