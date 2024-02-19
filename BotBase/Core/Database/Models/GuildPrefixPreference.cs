namespace BotBase.Database;

#nullable disable
public class GuildPrefixPreference : DbModel
{
    public ulong GuildId { get; set; }
    public string Prefix { get; set; }
}