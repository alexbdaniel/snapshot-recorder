using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    [JsonConverter(typeof(OutputFileExtensionConverter))]
    public OutputFileExtension OutputImageFormat { get; init; } = OutputFileExtension.Bmp;

    public OutputFormat OutputFormat => new OutputFormat(OutputImageFormat);
}