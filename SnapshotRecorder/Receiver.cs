using System.Diagnostics.CodeAnalysis;

namespace SnapshotRecorder;

public class Receiver
{
    private readonly DirectoryInfo saveDirectory;
    private readonly string cameraName;

    public Receiver(DirectoryInfo saveDirectory, string cameraName)
    {
        string directoryName = Path.Combine(saveDirectory.FullName, cameraName);
        this.saveDirectory = Directory.CreateDirectory(directoryName);
        this.cameraName = cameraName;
    }

    [SuppressMessage("ReSharper", "EventUnsubscriptionViaAnonymousDelegate")]
    public async Task ReceiveAsync(string sourceUri, CancellationToken cancellationToken = default)
    {
        Uri uri = new Uri(sourceUri);
        var inputSource = new StreamInputSource(uri);
        
        var client = new VideoStreamClient();

        client.NewImageReceived += bytes => _ = HandleNewImageReceivedAsync(bytes);
        
        await client.StartFrameReaderAsync(inputSource, OutputImageFormat.Bmp, cancellationToken);
        
        client.NewImageReceived -= bytes => _ = HandleNewImageReceivedAsync(bytes);
    }

    private async Task HandleNewImageReceivedAsync(byte[] imageData)
    {
        string filePath = Path.Combine(saveDirectory.FullName, $"{DateTime.UtcNow:yyyyMMddTHHmmssfffK}-{cameraName}.bmp");

        await File.WriteAllBytesAsync(filePath, imageData);
    }
    

}