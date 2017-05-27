using NReco.VideoConverter;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TheShowsConverter
{
    public partial class ChenIsStupid : Form
    {
        private const string LOG_PATH = "c:\\logs\\theShowsConverter.log";
        private readonly string _cid;

        public ChenIsStupid()
        {
            InitializeComponent();

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;

            _cid = Guid.NewGuid().ToString();

            Debugger.Launch();

            using (StreamWriter w = File.AppendText(LOG_PATH))
            {
                try
                {
                    Log(w, "Program started");

                    var directory = GetDirectory(w);

                    ConvertDirectory(w, directory);
                }
                catch (Exception ex)
                {
                    Log(w, $"FAILURE! ex: {ex}");
                }
            }
        }

        private string GetDirectory(StreamWriter w)
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                var directory = args[1];

                Log(w, $"Params recived. directory: {directory}");

                return directory;
            }
            catch (Exception ex)
            {
                throw new Exception("Getting params failed.", ex);
            }
        }

        private void ConvertDirectory(StreamWriter w, string path)
        {
            var files = Directory.GetFiles(path, "*.mkv", SearchOption.AllDirectories);

            var converter = new FFMpegConverter();

            foreach (var fileName in files)
            {
                ConvertFile(w, converter, path, fileName);
            }
        }

        private void ConvertFile(StreamWriter w, FFMpegConverter converter, string path, string fileName)
        {
            try
            {
                var mkvPath = Path.Combine(path, fileName);
                var mp4Path = Path.ChangeExtension(mkvPath, "mp4");

                converter.ConvertMedia(mkvPath, mp4Path, Format.mp4);
            }
            catch (Exception ex)
            {
                Log(w, $"FAILURE! exception occured while converting the file {fileName}. ex: {ex}");
            }
        }

        private void Log(StreamWriter w, string message)
        {
            w.WriteLine($"{_cid} | {DateTime.Now.ToString()} | {message}");
        }
    }
}
