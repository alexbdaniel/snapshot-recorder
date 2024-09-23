namespace SnapshotRecorder;

public class Receiver
{
    private readonly DirectoryInfo saveDirectory;
    private readonly string cameraName;

    public Receiver(DirectoryInfo saveDirectory, string cameraName)
    {
        this.saveDirectory = saveDirectory;
        this.cameraName = cameraName;
    }

    public async Task ReceiveAsync(string sourceUri, CancellationToken cancellationToken = default)
    {
        Uri uri = new Uri(sourceUri);
        var inputSource = new StreamInputSource(uri);
        
        var client = new VideoStreamClient();

        client.NewImageReceived += bytes => NewImageReceived(bytes);
        
        client.NewImageReceived += NewImageReceived;
        await client.StartFrameReaderAsync(inputSource, OutputImageFormat.Bmp, cancellationToken);
        
        client.NewImageReceived -= NewImageReceived;
    }

    private void NewImageReceived(byte[] imageData)
    {
        // Task.Delay(TimeSpan.FromSeconds(5));
        
        string directoryName = Path.Combine(saveDirectory.FullName, cameraName);
        Directory.CreateDirectory(directoryName);

        string filePath = Path.Combine(directoryName, $"{DateTime.UtcNow:yyyyMMddTHHmmssfffK}-{cameraName}.bmp");
        
        File.WriteAllBytes(filePath, imageData);
    }
}