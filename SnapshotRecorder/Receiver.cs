using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotRecorder.Configuration;

namespace SnapshotRecorder;

public class Receiver
{
    private readonly ConfigurationOptions configurationOptions;
    private readonly ILogger logger;

    public Receiver(IOptions<ConfigurationOptions> configurationOptions, ILogger<Receiver> logger)
    {
        this.configurationOptions = configurationOptions.Value;
        this.logger = logger;
    }
    
    [SuppressMessage("ReSharper", "EventUnsubscriptionViaAnonymousDelegate")]
    public async Task ReceiveAsync(CaptureOptions captureOptions, string cameraName, CancellationToken cancellationToken = default)
    {
        var inputSource = new StreamInputSource(captureOptions.StreamUri);
        
        var client = new VideoStreamClient(logger, configurationOptions.FfmpegFilePath);

        // client.NewImageReceived += HandleNewImageReceivedAsync;
        
        client.NewImageReceived += image => HandleNewImageReceivedAsync(image, captureOptions, cameraName); 
        
        await client.StartFrameReaderAsync(inputSource, captureOptions.OutputFormat, cancellationToken, false, captureOptions.Fps);

        client.NewImageReceived -= image => HandleNewImageReceivedAsync(image, captureOptions, cameraName); 
    }

    private async void HandleNewImageReceivedAsync(byte[] image, CaptureOptions captureOptions, string cameraName)
    {
        string directoryName = Path.Combine(captureOptions.SaveRootDirectoryName, captureOptions.SaveBaseDirectoryName);
        DirectoryInfo saveDirectory = Directory.CreateDirectory(directoryName);

        string extension = captureOptions.OutputFormat.FileExtension;
        string fullPath = Path.Combine(saveDirectory.FullName, $"{DateTime.UtcNow:yyyyMMddTHHmmssfffK}-{cameraName}.{extension}");

        try
        {
            await File.WriteAllBytesAsync(fullPath, image);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "An IO exception occured.");
        }
    }
    
    

}