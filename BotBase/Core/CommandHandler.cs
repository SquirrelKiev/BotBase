using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace BotBase
{
    public class CommandHandler
    {
        protected readonly InteractionService interactionService;
        protected readonly CommandService commandService;
        protected readonly DiscordSocketClient client;
        protected readonly BotConfigBase botConfig;
        protected readonly IServiceProvider services;

        public CommandHandler(InteractionService interactionService, CommandService commandService, DiscordSocketClient client, BotConfigBase botConfig, IServiceProvider services)
        {
            this.interactionService = interactionService;
            this.commandService = commandService;
            this.client = client;
            this.botConfig = botConfig;
            this.services = services;
        }

        public async Task OnReady(params Assembly[] assemblies)
        {
            await InitializeInteractionService(assemblies);
            await InitializeCommandService(assemblies);
        }

        #region Prefix Command Handling

        protected async Task MessageReceived(SocketMessage msg)
        {
            if (msg.Author.IsBot)
                return;

            if (msg is not SocketUserMessage userMessage)
                return;

            try
            {
                await RunCommand(userMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Command failed: ");
            }
        }

        protected async Task RunCommand(SocketUserMessage userMessage)
        {
            var prefix = await GetPrefix(userMessage.Channel);

            var argPos = 0;
            if (!userMessage.HasStringPrefix(prefix, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(client, userMessage);

            await commandService.ExecuteAsync(context, argPos, services);
        }

        protected virtual Task<string> GetPrefix(ISocketMessageChannel? channel)
        {
            return Task.FromResult(botConfig.DefaultPrefix);
        }

        protected async Task CommandExecuted(Optional<CommandInfo> cmdInfoOpt, ICommandContext ctx, Discord.Commands.IResult res)
        {
            var cmdInfo = cmdInfoOpt.IsSpecified ? cmdInfoOpt.Value : null;

            if (res.IsSuccess)
            {
                Log.Information("Command {ModuleName}.{MethodName} successfully executed. Message contents: {contents}",
                    cmdInfo?.Module.Name, cmdInfo?.Name, ctx.Message.CleanContent);
            }
            else
            {
                if (res.Error == CommandError.UnknownCommand)
                    return;

                if (res is Discord.Commands.ExecuteResult executeResult)
                {
                    Log.Error(executeResult.Exception, "Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}. Message contents: {contents}",
                        cmdInfo?.Module?.Name, cmdInfo?.Name, executeResult.Error, executeResult.ErrorReason, ctx.Message.CleanContent);
                }
                else
                {
                    Log.Error("Command {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}. Message contents: {contents}",
                        cmdInfo?.Module?.Name, cmdInfo?.Name, res.Error, res.ErrorReason, ctx.Message.CleanContent);
                }

                try
                {
                    IEmote emote;

                    if (Emote.TryParse(botConfig.ErrorEmote, out var result))
                    {
                        emote = result;
                    }
                    else
                    {
                        emote = Emoji.Parse(botConfig.ErrorEmote);
                    }


                    await ctx.Message.AddReactionAsync(emote);

                }
                catch (Exception e)
                {
                    Log.Warning(e, "Failed to add the error reaction!");
                }
            }
        }

        #endregion

        #region Interaction Handling

        protected Task InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, Discord.Interactions.IResult res)
        {
            if (res.IsSuccess)
            {
                Log.Information("Interaction {ModuleName}.{MethodName} successfully executed.", cmdInfo.Module.Name, cmdInfo.MethodName);
            }
            else
            {
                if (res is Discord.Interactions.ExecuteResult executeResult)
                {
                    Log.Error(executeResult.Exception, "Interaction {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.MethodName, executeResult.Error, executeResult.ErrorReason);
                }
                else
                {
                    Log.Error("Interaction {ModuleName}.{MethodName} failed. {Error}, {ErrorReason}.",
                        cmdInfo?.Module?.Name, cmdInfo?.MethodName, res.Error, res.ErrorReason);
                }

                var messageBody = $"{res.Error}, {res.ErrorReason}";

                if (res is Discord.Interactions.PreconditionResult precondResult)
                {
                    messageBody = $"Condition to use interaction not met. (`{precondResult.ErrorReason}`)";
                }

                if (ctx.Interaction.HasResponded)
                {
                    ctx.Interaction.ModifyOriginalResponseAsync(new MessageContents(messageBody, embed: null, null));
                }
                else
                {
                    ctx.Interaction.RespondAsync(messageBody, ephemeral: true);
                }
            }

            return Task.CompletedTask;
        }


        protected async Task InteractionCreated(SocketInteraction arg)
        {
            var ctx = new SocketInteractionContext(client, arg);

            if (ctx.Interaction is SocketMessageComponent componentInteraction)
            {
                var ogRes = componentInteraction.Message;

                var ogAuthor = ogRes.Interaction?.User.Id;

                // horrible
                if (ogAuthor == null)
                {
                    var channel = (ISocketMessageChannel)await client.GetChannelAsync(ogRes.Reference.ChannelId);
                    var message = await channel.GetMessageAsync(ogRes.Reference.MessageId.Value);
                    ogAuthor = message?.Author?.Id;
                }

                if (ogAuthor != null && ogAuthor != ctx.Interaction.User.Id)
                {
                    await componentInteraction.RespondAsync("You did not originally trigger this. Please run the command yourself.", ephemeral: true);

                    return;
                }
            }

            await interactionService.ExecuteCommandAsync(ctx, services);
        }

        #endregion

        protected async Task InitializeInteractionService(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies.Append(Assembly.GetExecutingAssembly()))
            {
                var modules = await interactionService.AddModulesAsync(assembly, services);

                foreach (var moduleInfo in modules)
                {
                    Log.Verbose("Registered Interaction Module: {moduleName}", moduleInfo.Name);
                }
            }

            await interactionService.RegisterCommandsGloballyAsync();

            client.InteractionCreated += InteractionCreated;
            interactionService.InteractionExecuted += InteractionExecuted;
        }

        protected async Task InitializeCommandService(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies.Append(Assembly.GetExecutingAssembly()))
            {
                var modules = await commandService.AddModulesAsync(assembly, services);

                foreach (var moduleInfo in modules)
                {
                    Log.Verbose("Registered Prefix Module: {moduleName}", moduleInfo.Name);
                }
            }

            client.MessageReceived += MessageReceived;
            commandService.CommandExecuted += CommandExecuted;
        }
    }
}
