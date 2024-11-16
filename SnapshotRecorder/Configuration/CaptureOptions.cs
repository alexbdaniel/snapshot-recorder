using System.ComponentModel.DataAnnotations;

namespace SnapshotRecorder.Configuration;

public class CaptureOptions
{
    [Required(AllowEmptyStrings = false)]
    public required Uri StreamUri { get; init; }
    
    public float Fps => 1 / CaptureInterval;
    
    // ReSharper disable once MemberCanBePrivate.Global
    public float CaptureInterval { get; init; } = 5;
    
    [Required(AllowEmptyStrings = false)]
    public required string SaveBaseDirectoryName { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string SaveRootDirectoryName { get; init; }
    
    public OutputImageFormat OutputImageFormat { get; init; } = OutputImageFormat.Bmp;
}