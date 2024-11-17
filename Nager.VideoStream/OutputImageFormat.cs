using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnapshotRecorder;

public enum OutputImageFormat
{
    MJpeg,
    Png,
    Bmp,
}

public enum OutputFileExtension
{
    Jpeg,
    Jpg,
    Png,
    Bmp
}

public class OutputFormat
{
    
    public string FileExtension =>
        chosenFileExtension switch
        {
            OutputFileExtension.Jpg => "jpeg",
            _ => chosenFileExtension.ToString().ToLower()
        };

    
    private readonly OutputFileExtension chosenFileExtension;
    
    public string FfmpegCodec =>
         chosenFileExtension switch
        {
            OutputFileExtension.Jpeg => "mjpeg",
            OutputFileExtension.Jpg => "mjpeg",
            _ => chosenFileExtension.ToString().ToLower()
        };
    
    public OutputFormat(OutputFileExtension fileExtension) =>
        this.chosenFileExtension = fileExtension;
}

public class OutputFileExtensionConverter : JsonConverter<OutputFileExtension?>
{
    public override OutputFileExtension? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (value == null)
            return null;

        bool parsed = Enum.TryParse(value, true, out OutputFileExtension fileExtension);
        if (parsed)
            return fileExtension;

        return null;
    }

    public override void Write(Utf8JsonWriter writer, OutputFileExtension? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
