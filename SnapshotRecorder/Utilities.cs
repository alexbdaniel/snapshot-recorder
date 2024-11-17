using System.Runtime.Versioning;

namespace SnapshotRecorder;

public static class Utilities
{
    /// <summary>
    /// Gets the avaible free disk space for the drive for the specified directory.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns>Available free space in bytes.</returns>
    public static long GetDiskSpace(DirectoryInfo directory) =>
        new DriveInfo(directory.FullName).AvailableFreeSpace;
}