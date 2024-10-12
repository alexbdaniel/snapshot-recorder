using System.Diagnostics.CodeAnalysis;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

public class Receiver
{
    private readonly DirectoryInfo saveDirectory;
    private readonly string cameraName;
    private readonly CaptureOptions captureOptions;
    private readonly ConfigurationOptions configurationOptions;

    public Receiver(CaptureOptions captureOptions, string cameraName, ConfigurationOptions configurationOptions)
    {
        string directoryName = Path.Combine(captureOptions.SaveRootDirectoryName, captureOptions.SaveBaseDirectoryName);
        saveDirectory = Directory.CreateDirectory(directoryName);
        this.captureOptions = captureOptions;
        this.cameraName = cameraName;
        this.configurationOptions = configurationOptions;
    }
    
    [SuppressMessage("ReSharper", "EventUnsubscriptionViaAnonymousDelegate")]
    public async Task ReceiveAsync(Uri source, CancellationToken cancellationToken = default)
    {
      
        var inputSource = new StreamInputSource(source);
        
        var client = new VideoStreamClient(configurationOptions.FfmpegFilePath);

        client.NewImageReceived += bytes => _ = HandleNewImageReceivedAsync(bytes);
        
        await client.StartFrameReaderAsync(inputSource, OutputImageFormat.Bmp, cancellationToken, false, captureOptions.Fps);
        
        client.NewImageReceived -= bytes => _ = HandleNewImageReceivedAsync(bytes);
    }
    
    private async Task HandleNewImageReceivedAsync(byte[] imageData)
    {
        string filePath = Path.Combine(saveDirectory.FullName, $"{DateTime.UtcNow:yyyyMMddTHHmmssfffK}-{cameraName}.bmp");

        await File.WriteAllBytesAsync(filePath, imageData);
    }
    

}