using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TheShowsConverter
{
    public partial class ChenIsStupid : Form
    {
        private static readonly IEnumerable<string> ConvertFromExtensionTypes = new[] {"*.mkv", "*.avi"};

        public ChenIsStupid()
        {
            InitializeComponent();

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Hide();
            string logFolder = @"c:\logs\IdanMorHamor\";
            var path = Path.Combine(logFolder, $"{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.log");

            Directory.CreateDirectory(logFolder); 
            using (var logger = File.AppendText(path))
            {
                try
                {
                    Log(logger, "Program started");

                    var files = GetFiles(logger);

                    ConvertFiles(logger, files);
                }
                catch (Exception ex)
                {
                    Log(logger, $"FAILURE! ex: {ex}");
                }
                finally
                {
                    Close();
                }
            }
        }

        private IEnumerable<string> GetFiles(TextWriter logger)
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                Log(logger,$"args are : {String.Join(",",args)}");
                var kind = args[1];
                var fileName = args[3];
                var directoryName = args[2];

                if (kind == "single")
                {
                    var filePath = Path.Combine(directoryName, fileName);
                    Log(logger, $"Single file selected. {filePath}");
                    return new[] { filePath };
                }

                var files = ConvertFromExtensionTypes.SelectMany(f => Directory.GetFiles(directoryName, f,
                    SearchOption.AllDirectories));
                Log(logger, $"Multi file selected. directory: {directoryName}. files: {string.Join(", ", files)}");
                return files;
            }
            catch (Exception ex)
            {
                throw new Exception("Getting params failed.", ex);
            }
        }

        private void ConvertFiles(TextWriter logger, IEnumerable<string> files)
        {
            var converter = new FFMpegConverter();
            converter.ConvertProgress += (o, p) => { Log(logger, $"Processed {p.Processed} of {p.TotalDuration}"); };
            converter.LogReceived += (o, p) => { Log(logger, $"ffmpeg: {p.Data}"); };
            foreach (var filePath in files)
            {
                ConvertFile(logger, converter, filePath);
            }
        }

        private void ConvertFile(TextWriter logger, FFMpegConverter converter, string filePath)
        {
            try
            {
                var mp4Path = Path.ChangeExtension(filePath, "mp4");

                converter.ConvertMedia(filePath, mp4Path, Format.mp4);

                //File.Delete(mkvPath);
            }
            catch (Exception ex)
            {
                Log(logger, $"FAILURE! exception occured while converting the file {filePath}. ex: {ex}");
            }
        }

        private void Log(TextWriter logger, string message)
        {
            logger.WriteLine($"{DateTime.Now} | {message}");
        }
    }
}
