namespace SnapshotRecorder.Configuration;

public class ReceiverOptions
{
    public const string Key = "Receiver";
    
    public required Dictionary<string, CaptureOptions> Cameras { get; init; }
}