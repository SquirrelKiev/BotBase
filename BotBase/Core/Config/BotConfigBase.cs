using Discord;
using Serilog;
using YamlDotNet.Serialization;

namespace BotBase;

public abstract class BotConfigBase
{
    [YamlMember(Description = @"Your bot token from https://discord.com/developers/applications. Don't share!")]
    public abstract string BotToken { get; set; }

    [YamlMember(Description = "The type of cache to use.\n" +
                              "Options are currently only \"Memory\".")]
    public abstract CacheType Cache { get; set; }
    
    [YamlMember(Description = "The type of database to use.\n" +
                              "Options are \"Sqlite\" and \"Postgresql\".")]
    public abstract DatabaseType Database { get; set; }

    [YamlMember(Description = "The connection string for the database specified above.\n" +
                            "Example Postgres string: Host=127.0.0.1;Username=postgres;Password=;Database=botdb\n" +
                            "Example Sqlite string: Data Source=data/BotDb.db")]
    public abstract string DatabaseConnectionString { get; set; }

    [YamlMember(Description = "The reaction to put on prefix commands when an unhandled error occurs. Will only appear on prefix commands.")]
    public abstract string ErrorEmote { get; set; }

    [YamlMember(Description = "A set of UserIDs. Users in this set will be granted permission to use commands to manage the instance itself.\n" +
                              "This is a dangerous permission to grant.")]
    public abstract HashSet<ulong> ManagerUserIds { get; set; }

    [YamlMember(Description = "An optional URL to an instance of Seq. Empty string is interpreted as not wanting Seq.")]
    public abstract string SeqUrl { get; set; }

    [YamlMember(Description = "An optional API key for Seq. Empty string is interpreted as no API key.")]
    public abstract string SeqApiKey { get; set; }

    [YamlMember(Description = "The default config for the bot.")]
    public abstract string DefaultPrefix { get; set; }

    /// <summary>
    /// For any string here, the following will be replaced:
    /// - {{guilds}} will be substituted with how many guilds (servers) the bot is in.
    /// - {{botUsername}} will be substituted with the bot's username.
    /// </summary>
    [YamlMember(Description = "***** ABOUT PAGE *****\n" +
                              "For any string here, the following will be replaced:\n" +
                              "- {{guilds}} will be substituted with how many guilds (servers) the bot is in.\n" +
                              "- {{botUsername}} will be substituted with the bot's username.\n" +
                              "\n" +
                              "The about page title.")]
    public abstract string AboutPageTitle { get; set; }

    /// <summary>
    /// For any string here, the following will be replaced:
    /// - {{guilds}} will be substituted with how many guilds (servers) the bot is in.
    /// - {{botUsername}} will be substituted with the bot's username.
    /// </summary>
    [YamlMember(Description = "The about page description.")]
    public abstract string AboutPageDescription { get; set; }

    [YamlMember(Description = "Fields within the about page.")]
    public abstract AboutField[] AboutPageFields { get; set; }

    public struct AboutField
    {
        /// <summary>
        /// For any string here, the following will be replaced:
        /// - {{guilds}} will be substituted with how many guilds (servers) the bot is in.
        /// - {{botUsername}} will be substituted with the bot's username.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// For any string here, the following will be replaced:
        /// - {{guilds}} will be substituted with how many guilds (servers) the bot is in.
        /// - {{botUsername}} will be substituted with the bot's username.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// For any string here, the following will be replaced:
        /// - {{guilds}} will be substituted with how many guilds (servers) the bot is in.
        /// - {{botUsername}} will be substituted with the bot's username.
        /// </summary>
        public bool Inline { get; set; }
    }

    public enum CacheType
    {
        Memory
    }

    public enum DatabaseType
    {
        Sqlite,
        Postgresql
    }

    public virtual bool IsValid()
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, BotToken);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Supplied bot token is invalid.");
            return false;
        }

        return true;
    }
}
