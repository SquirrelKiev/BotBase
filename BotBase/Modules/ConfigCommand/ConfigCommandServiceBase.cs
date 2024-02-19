using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;

namespace BotBase.Modules.ConfigCommand;

public class ConfigCommandServiceBase<T> where T : Enum
{
    public struct State
    {
        public T page;
        public string data;

        public State()
        {
            page = default;
            data = string.Empty;
        }

        public State(T page) : this()
        {
            this.page = page;
            data = string.Empty;
        }

        public State(T page, string data)
        {
            this.page = page;
            this.data = data;
        }
    }

    public Dictionary<T, ConfigPageBase<T>> ConfigPages 
    { 
        get
        {
            return configPages ??= GetConfigPages(services);
        }
    }
    private Dictionary<T, ConfigPageBase<T>>? configPages;

    private readonly IServiceProvider services;

    public ConfigCommandServiceBase(IServiceProvider services)
    {
        this.services = services;
    }

    public async Task<MessageContents> GetMessageContents(State state, IInteractionContext context)
    {
        var page = ConfigPages[state.page];

        var method = page.GetType().GetMethod("SetContext", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new NullReferenceException("SetContext doesnt exist!");
        method.Invoke(page, new object[] { context });

        return await page.GetMessageContents(state);
    }

    public static Dictionary<T, ConfigPageBase<T>> GetConfigPages(IServiceProvider services)
    {
        return services.GetServices<ConfigPageBase<T>>().ToDictionary(type => type.Id);
    }
}
