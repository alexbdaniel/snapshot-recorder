using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SnapshotRecorder.Configuration;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, HostBuilderContext context)
    {
        services.ConfigureOptions(context);

        services.AddHostedService<CaptureService>();
        
        return services;
    }
    
    private static IServiceCollection ConfigureOptions(this IServiceCollection services, HostBuilderContext context)
    {
        services.AddOptions<ReceiverOptions>().Bind(context.Configuration.GetSection(ReceiverOptions.Key));

        services.AddOptions<ConfigurationOptions>().Bind(context.Configuration.GetSection(ConfigurationOptions.Key))
            .ValidateOnStart();
        
        return services;
    }
}