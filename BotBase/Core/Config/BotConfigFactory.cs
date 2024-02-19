using System.Diagnostics.CodeAnalysis;
using Serilog;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BotBase;

public class BotConfigFactory<T> where T : BotConfigBase, new()
{
    private readonly string configPath = Environment.GetEnvironmentVariable("BOT_CONFIG_LOCATION") ??
                                         Path.Combine(Path.Combine(AppContext.BaseDirectory, "data"), "bot_config.yaml");

    public bool GetConfig([NotNullWhen(true)] out T? botConfig)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted)
            .Build();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        if (!File.Exists(configPath))
        {
            botConfig = new T();

            Log.Fatal("Config not found. Creating new config at {ConfigPath}. Please edit this file and restart the bot.", configPath);

            var dirName = Path.GetDirectoryName(configPath);
            if (dirName != null)
                Directory.CreateDirectory(dirName);

            File.WriteAllText(configPath, serializer.Serialize(botConfig));
            return false;
        }

        try
        {
            botConfig = deserializer.Deserialize<T>(File.ReadAllText(configPath));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to read config.");
            botConfig = new T();
            return false;
        }

        //#if DEBUG
        //botConfig.GenerateMetadata();
        File.WriteAllText(configPath, serializer.Serialize(botConfig));
        //#endif

        return true;

    }
}
