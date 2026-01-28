using System.Text;
using System.IO;

namespace ExportConfig
{
    public class PNGExport
    {
        int imageWidth;
        int imageHeight;
        string fileName;

        public PNGExport(string fileName) { 
            // Resolution by default
            this.imageWidth = 256;
            this.imageHeight = 256;
            this.fileName = fileName;
        }
        public PNGExport(int imageWidth, int imageHeight, string fileName)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.fileName = fileName;
        }

        public void ExportFile(StringBuilder data) {

            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = Path.Combine(userProfilePath, "Downloads");
            Directory.CreateDirectory(downloadsPath);
            string filePath = Path.Combine(downloadsPath, $"{fileName}.ppm");

            using StreamWriter writer = new StreamWriter(filePath);

            writer.WriteLine("P3");
            writer.WriteLine($"{imageWidth} {imageHeight}");
            writer.WriteLine("255");

            // Writing data
            writer.Write(data.ToString());

            Console.WriteLine($"Render completado, archivo '{fileName}.ppm' creado");
        }
    }
}
