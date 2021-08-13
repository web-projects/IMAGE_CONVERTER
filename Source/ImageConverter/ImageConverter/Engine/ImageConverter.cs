using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ImageConverter.Engine
{
    public class ImageConverterEngine
    {
        readonly string SignatureFilename = "signature.png";
        readonly string filePath;
        readonly string fileName;

        public ImageConverterEngine()
        {
            filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "output");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            fileName = Path.Combine(filePath, SignatureFilename);
        }

        public bool ConvertByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }

            return File.Exists(fileName);
        }

        public void ImageDisplay()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            ProcessStartInfo psi = new ProcessStartInfo("rundll32.exe",
                String.Format("\"{0}{1}\", ImageView_Fullscreen {2}",
                    Environment.Is64BitOperatingSystem ? path.Replace(" (x86)", "") : path,
                    @"\Windows Photo Viewer\PhotoViewer.dll",
                    fileName));

            psi.UseShellExecute = false;

            Process viewer = Process.Start(psi);

            // cleanup when done...
            viewer.EnableRaisingEvents = true;
            viewer.Exited += (o, args) =>
            {
                File.Delete(fileName);
            };
        }
    }
}
