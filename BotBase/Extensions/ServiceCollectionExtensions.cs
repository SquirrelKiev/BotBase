using Microsoft.Extensions.DependencyInjection;

namespace BotBase;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache<T>(this IServiceCollection services, T config) where T : BotConfigBase
    {
        switch (config.Cache)
        {
            case BotConfigBase.CacheType.Memory:
                services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
                break;
            default:
                throw new NotSupportedException(config.Cache.ToString());
        }
        
        return services;
    }
}
