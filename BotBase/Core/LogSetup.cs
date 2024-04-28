using Serilog.Sinks.SystemConsole.Themes;
using System.Text;
using Serilog;
using Serilog.Events;

namespace BotBase;

public static class LogSetup
{
    public static void SetupLogger(BotConfigBase config, LogEventLevel logLevel = LogEventLevel.Information)
    {
        var logConfig = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .Enrich.FromLogContext()
            .WriteTo.Console(
            logLevel,
            theme: AnsiConsoleTheme.Literate)
            ;

        if (!string.IsNullOrWhiteSpace(config.SeqUrl))
        {
            logConfig.WriteTo.Seq(config.SeqUrl, apiKey: config.SeqApiKey);
        }

        Log.Logger = logConfig.CreateLogger();

        Console.OutputEncoding = Encoding.UTF8;
    }
}
