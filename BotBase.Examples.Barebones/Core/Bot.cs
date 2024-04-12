using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace BotBase.Examples.Barebones;

public class Bot
{
    public DiscordSocketClient Client { get; }
    public InteractionService InteractionService { get; }
    public CommandService CommandService { get; }
    public BotConfig Config { get; }

    private readonly IServiceProvider services;

    public Bot(BotConfig config)
    {
        Config = config;

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.MessageContent |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.DirectMessages,
            LogLevel = LogSeverity.Verbose
        });
        InteractionService = new InteractionService(Client, new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Verbose,
            DefaultRunMode = Discord.Interactions.RunMode.Async
        });
        CommandService = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Verbose,
            DefaultRunMode = Discord.Commands.RunMode.Async
        });

        services = CreateServices();
        Log.Information("Services created.");
    }

    private ServiceProvider CreateServices()
    {
        var collection = new ServiceCollection()
            .AddCache(Config)
            .AddSingleton<BotConfigBase>(Config)
            .AddSingleton(Config)
            .AddSingleton(Client)
            .AddSingleton(InteractionService)
            .AddSingleton(CommandService)
            .AddSingleton<CommandHandler>();

        collection.Scan(scan => scan.FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.WithAttribute<InjectAttribute>(x =>
                x.ServiceLifetime == ServiceLifetime.Singleton)
            )
            .AsSelf()
            .WithSingletonLifetime()
        );

        collection.Scan(scan => scan.FromAssemblyOf<Bot>()
            .AddClasses(classes => classes.WithAttribute<InjectAttribute>(x =>
                x.ServiceLifetime == ServiceLifetime.Transient)
            )
            .AsSelf()
            .WithTransientLifetime()
        );

        return collection.BuildServiceProvider();
    }

    public async Task RunAndBlockAsync()
    {
        Log.Information("Starting bot...");
        await RunAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private async Task RunAsync()
    {
        Client.Log += Client_Log;

        Client.Ready += Client_Ready;

        await Client.LoginAsync(TokenType.Bot, Config.BotToken);
        await Client.StartAsync();
    }

    private Task Client_Log(LogMessage message)
    {
        var level = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information,
        };

        if (message.Exception is not null)
        {
            Log.Write(level, message.Exception, "{Source} | {Message}", message.Source, message.Message);
        }
        else
        {
            Log.Write(level, "{Source} | {Message}", message.Source, message.Message);
        }
        return Task.CompletedTask;
    }

    private async Task Client_Ready()
    {
        Log.Information("Logged in as {user}#{discriminator} ({id})", Client.CurrentUser?.Username, Client.CurrentUser?.Discriminator, Client.CurrentUser?.Id);

        await services.GetRequiredService<CommandHandler>().OnReady(Assembly.GetExecutingAssembly());
    }
}
