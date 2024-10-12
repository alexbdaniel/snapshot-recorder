using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

public class CaptureService : BackgroundService
{
    private readonly ReceiverOptions receiverOptions;
    private readonly ConfigurationOptions configurationOptions;

    public CaptureService(IOptions<ReceiverOptions> options, IOptions<ConfigurationOptions> configurationOptions)
    {
        this.configurationOptions = configurationOptions.Value;
        this.receiverOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var tasks = GetTasks(stoppingToken);

        await Parallel.ForEachAsync(receiverOptions.Cameras, stoppingToken, async (pair, cancellationToken) =>
        {
            string cameraName = pair.Key;
            var captureOptions = pair.Value;
            
            var receiver = new Receiver(captureOptions, cameraName, configurationOptions);
            await receiver.ReceiveAsync(captureOptions.StreamUri, cancellationToken);
        });

    }

    private List<Task> GetTasks(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        foreach (var camera in receiverOptions.Cameras)
        {
            string directoryName = Path.Combine(camera.Value.SaveRootDirectoryName, camera.Value.SaveBaseDirectoryName);
            var saveDirectory = new DirectoryInfo(camera.Value.SaveRootDirectoryName);
            var directory = new DirectoryInfo(directoryName); 
            
            var receiver = new Receiver(camera.Value, camera.Key, configurationOptions);
            var task = new Task(() => _ = receiver.ReceiveAsync(camera.Value.StreamUri, cancellationToken));
            
            tasks.Add(task);

        }

        return tasks;
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping...");

        await base.StopAsync(cancellationToken);
    }
}