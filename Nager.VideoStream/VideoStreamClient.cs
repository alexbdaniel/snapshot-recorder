using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SnapshotRecorder
{
    public class VideoStreamClient
    {
        public event Action<byte[]>? NewImageReceived;
        public event Action<string>? FFmpegInfoReceived;
        
        private readonly ILogger logger;

        private readonly string exePath;

        public VideoStreamClient(ILogger logger, string exePath = "ffmpeg.exe")
        {
            if (!File.Exists(exePath))
            {
                throw new FileNotFoundException("Cannot found ffmpeg", exePath);
            }

            this.logger = logger;
            this.exePath = exePath;
        }

        /// <summary>
        /// Start Frame reader
        /// </summary>
        /// <param name="inputSource">The source of the image frames</param>
        /// <param name="outputFormat"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="useShellExecute">Use the Operating System shell to start the process</param>
        /// <param name="fps">Frames per second to capture</param>
        /// /// <returns></returns>
        public async Task StartFrameReaderAsync(
            InputSource inputSource,
            OutputFormat outputFormat,
            CancellationToken cancellationToken = default,
            bool useShellExecute = false,
            float fps = 7)
        {
            //-rtsp_transport tcp
            //-qscale:v 5
            // string inputArgs = $"-y {inputSource.InputCommand}";
            string inputArgs = $" -y {inputSource.InputCommand}";
            string outputArgs = $" -qscale:v 2 -r {fps} -c:v {outputFormat.FfmpegCodec} -f image2pipe -";
            
            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"{inputArgs} {outputArgs}",
                UseShellExecute = useShellExecute,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (var ffmpegProcess = new Process { StartInfo = startInfo })
            {
                ffmpegProcess.ErrorDataReceived += this.ProcessDataReceived;

                ffmpegProcess.OutputDataReceived += HandleProcessOutput; 
                
                
                ffmpegProcess.Start();
                ffmpegProcess.BeginErrorReadLine();

                using (var frameOutputStream = ffmpegProcess.StandardOutput.BaseStream)
                {
                    var index = 0;
                    var buffer = new byte[32768];
                    var imageData = new List<byte>();
                    byte[]? imageHeader = null;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var length = await frameOutputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (length == 0)
                        {
                            break;
                        }

                        //Set Image Header with first data
                        if (imageHeader == null)
                        {
                            imageHeader = buffer.Take(5).ToArray();
                        }

                        if (buffer.Take(5).SequenceEqual(imageHeader))
                        {
                            if (imageData.Count > 0)
                            {
                                this.NewImageReceived?.Invoke(imageData.ToArray());
                                imageData.Clear();
                                index++;
                            }
                        }

                        imageData.AddRange(buffer.Take(length));
                    }

                    frameOutputStream.Close();
                }

              
                ffmpegProcess.ErrorDataReceived -= this.ProcessDataReceived;
                ffmpegProcess.OutputDataReceived -= HandleProcessOutput; 

                ffmpegProcess.WaitForExit(1000);

                if (!ffmpegProcess.HasExited)
                {
                    ffmpegProcess.Kill();
                }
            }
        }

        private void HandleProcessOutput(object sender, DataReceivedEventArgs e)
        {
            logger.LogDebug(e.Data);
        }

        private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            logger.LogDebug(e.Data);
            // logger.LogError("VideoStreamClient error {eventArgs}", e);
            FFmpegInfoReceived?.Invoke(e.Data);

        }
    }
}
