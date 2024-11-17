using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

public class CaptureService : BackgroundService
{
    private readonly ReceiverOptions receiverOptions;
    private readonly ConfigurationOptions configurationOptions;
    private readonly ILogger logger;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public CaptureService(IOptions<ReceiverOptions> options, IOptions<ConfigurationOptions> configurationOptions, ILogger<CaptureService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
        this.configurationOptions = configurationOptions.Value;
        this.receiverOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = receiverOptions.Cameras;

        // options.TryGetValue(cameraName, out CaptureOptions? captureOptions);

        
        
        
        // var receiver = new Receiver(captureOptions!, cameraName, configurationOptions);
        // await receiver.ReceiveAsync(captureOptions, cameraName, stoppingToken);
        
        await Parallel.ForEachAsync(receiverOptions.Cameras, stoppingToken, async (pair, cancellationToken) =>
        {
            string cameraName = pair.Key;
            var captureOptions = pair.Value;
            
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var receiver = scope.ServiceProvider.GetRequiredService<Receiver>();
            
            await receiver.ReceiveAsync(captureOptions, cameraName, cancellationToken);
        });

    }

    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping...");

        await base.StopAsync(cancellationToken);
    }
}