using System.ComponentModel.DataAnnotations;

namespace BotBase.Database;

public abstract class DbModel
{
    [Key]
    public uint Id { get; set; }
}
