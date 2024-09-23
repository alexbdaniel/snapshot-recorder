namespace SnapshotRecorder;

internal static class Program
{
    
    
    private static async Task Main(string[] args)
    {
        string uri = "rtsp://admin:UnpickedTreach75@192.168.50.240:554/ch01/0";

        string saveDir = @"C:\Users\alex.daniel\Pictures\rtsp";
        var directory = new DirectoryInfo(saveDir);

        await new Receiver(directory, "front").ReceiveAsync(uri);



    }
    
}
