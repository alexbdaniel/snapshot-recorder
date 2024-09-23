using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

public class CaptureService : BackgroundService
{
    private readonly ReceiverOptions options;

    public CaptureService(IOptions<ReceiverOptions> options)
    {
        this.options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var tasks = GetTasks(stoppingToken);

        await Parallel.ForEachAsync(options.Cameras, stoppingToken, async (pair, cancellationToken) =>
        {
            string cameraName = pair.Key;
            var captureOptions = pair.Value;
            
            var saveDirectory = new DirectoryInfo(captureOptions.SaveRootDirectoryName);

            var receiver = new Receiver(captureOptions, cameraName);
            await receiver.ReceiveAsync(captureOptions.StreamUri, cancellationToken);
        });

    }

    private List<Task> GetTasks(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        foreach (var camera in options.Cameras)
        {
            string directoryName = Path.Combine(camera.Value.SaveRootDirectoryName, camera.Value.SaveBaseDirectoryName);
            var saveDirectory = new DirectoryInfo(camera.Value.SaveRootDirectoryName);
            var directory = new DirectoryInfo(directoryName); 
            
            var receiver = new Receiver(camera.Value, camera.Key);
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