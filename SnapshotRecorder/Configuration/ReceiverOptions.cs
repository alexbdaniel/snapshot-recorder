using System.ComponentModel.DataAnnotations;

namespace SnapshotRecorder.Configuration;

public class ReceiverOptions
{
    public const string Key = "Receiver";
    
    [Required]
    public required Dictionary<string, CaptureOptions> Cameras { get; init; }
}