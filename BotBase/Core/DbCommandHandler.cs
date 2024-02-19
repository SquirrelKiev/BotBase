using BotBase.Database;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace BotBase
{
    public class DbCommandHandler<T> : CommandHandler where T : BotDbContextBase
    {
        protected readonly DbServiceBase<T> dbService;

        public DbCommandHandler(InteractionService interactionService, CommandService commandService, 
            DiscordSocketClient client, BotConfigBase botConfig, IServiceProvider services, DbServiceBase<T> dbService) : 
            base(interactionService, commandService, client, botConfig, services)
        {
            this.dbService = dbService;
        }

        protected override async Task<string> GetPrefix(ISocketMessageChannel? channel)
        {
            var prefix = botConfig.DefaultPrefix;

            if (channel is SocketTextChannel textChannel)
            {
                prefix = await dbService.GetPrefix(textChannel.Guild.Id, botConfig.DefaultPrefix);
            }

            return prefix;
        }
    }
}
