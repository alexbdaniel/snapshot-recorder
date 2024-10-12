namespace SnapshotRecorder.Configuration;

public class ConfigurationOptions
{
    public const string Key = "Configuration";
    
    public string FfmpegFilePath { get; init; } = "ffmpeg.exe";
}