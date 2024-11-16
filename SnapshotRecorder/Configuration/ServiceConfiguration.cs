using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SnapshotRecorder.Configuration;

public static class ServiceConfiguration
{
    private static IServiceCollection ConfigureServices(this IServiceCollection services, HostApplicationBuilder builder)
    {
        services.ConfigureOptions(builder);

        services.AddHostedService<CaptureService>();
        
        return services;
    }

    public static IServiceCollection ConfigureSnapshotRecorder(this IServiceCollection services, HostApplicationBuilder builder)
    {
        services.ConfigureServices(builder);
        
        return services;
    }
    
    private static IServiceCollection ConfigureOptions(this IServiceCollection services, HostApplicationBuilder builder)
    {
        services.AddOptions<ReceiverOptions>().Bind(builder.Configuration.GetSection(ReceiverOptions.Key));

        services.AddOptions<ConfigurationOptions>().Bind(builder.Configuration.GetSection(ConfigurationOptions.Key))
            .ValidateOnStart();
        
        return services;
    }
    
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