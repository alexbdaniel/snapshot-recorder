namespace SnapshotRecorder.Configuration;

public class CaptureOptions
{
    public required Uri StreamUri { get; init; }
    
    public float Fps { get; init; } = 7;
    
    public required string SaveBaseDirectoryName { get; init; }
    
    public required string SaveRootDirectoryName { get; init; }
}