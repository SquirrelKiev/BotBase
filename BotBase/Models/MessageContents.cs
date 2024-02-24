using Discord;

namespace BotBase;

public struct MessageContents
{
    public string body;
    public Embed[]? embeds;
    public MessageComponent? components;

    public MessageContents(string body, Embed[] embeds, ComponentBuilder? components)
    {
        this.body = body;
        this.embeds = embeds;

        components ??= new ComponentBuilder().WithRedButton();
        this.components = components?.Build();
    }

    public MessageContents(string body = "", Embed? embed = null, ComponentBuilder? components = null)
    {
        this.body = body;
        embeds = embed == null ? null : new[] { embed };

        components ??= new ComponentBuilder().WithRedButton();

        this.components = components.Build();
    }

    public MessageContents(EmbedBuilder embed, ComponentBuilder? components = null, string body = "")
    {
        this.body = body;
        embeds = new[] { embed.Build() };

        components ??= new ComponentBuilder().WithRedButton();

        this.components = components.Build();
    }

    public MessageContents SetEmbed(Embed embed)
    {
        embeds = new[] { embed };

        return this;
    }
}
