using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

internal static class Program
{
    
    
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.development.json", true)
            .Build();

        IHostBuilder host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddConfiguration(configuration);
            })
            .ConfigureHostOptions(options =>
            {
                options.ServicesStartConcurrently = true;
                options.ServicesStopConcurrently = true;
            })
            .ConfigureServices((context, services) => 
                services.ConfigureServices(context));
        
        await host.RunConsoleAsync().ConfigureAwait(false);
        

    }
    
}
