using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

internal static class Program
{
    private static async Task Main()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .SetEnvironmentNameFromAppSettings(ref builder)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLower()}.json", true)
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .AddEnvironmentVariables();

        builder.Services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });
        
        var services = builder.Services;
        
        services.ConfigureServices(builder);
        
        IHost application = builder.Build();
        
        await application.RunAsync().ConfigureAwait(false);
    }
    
    private static IConfigurationBuilder SetEnvironmentNameFromAppSettings(this IConfigurationBuilder configurationManager, ref HostApplicationBuilder builder)
    {
        string environmentName = builder.Configuration
            .GetSection("Configuration")
            .GetValue<string>("Environment") ?? "Production";
        
        builder.Environment.EnvironmentName = environmentName;
        
        return configurationManager;
    }
    
}
