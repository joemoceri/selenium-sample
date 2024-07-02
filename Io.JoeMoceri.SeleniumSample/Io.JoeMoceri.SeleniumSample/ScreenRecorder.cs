using System;
using System.Diagnostics;

namespace SeleniumFFmpeg
{
    public interface IScreenRecorder
    {
        void StartRecording(string id);
        void StopRecording();
    }

    public class ScreenRecorder : IScreenRecorder, IDisposable
    {
        private Process process;

        public void Dispose()
        {
            StopRecording();
        }

        public void StartRecording(string id)
        {
            var timestamp = DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ss");
            var framerate = "10";
            var videoSize = "1920x1080";

            var processInfo = new ProcessStartInfo
            {
                Arguments = $"-y -f gdigrab -framerate {framerate} -video_size {videoSize} -i desktop output.{timestamp}.{id}.mp4",
                FileName = "C:/ffmpeg/bin/ffmpeg.exe",
                //RedirectStandardInput = true,
            };

            process = Process.Start(processInfo);
        }

        public void StopRecording()
        {
            process.StandardInput.Write("q");
            process.WaitForExit();
            process.Dispose();
        }
    }
}
