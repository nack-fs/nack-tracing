using System.Text;
using System.IO;
using ExportConfig;

namespace NackTracing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StringBuilder imageData = new StringBuilder();
            int imageWidth = 256;
            int imageHeight = 256;

            for (int y = 0; y < imageHeight; y++) {
                Console.Title = $"Processing line: {y} of {imageHeight}";
                for (int x = 0; x < imageWidth; x++) { 
                    var r = (double)x / (imageWidth - 1);
                    var g = (double)y / (imageHeight - 1);
                    var b = 0.0;

                    int ir = (int)(255.999 * r);
                    int ig = (int)(255.999 * g);
                    int ib = (int)(255.999 * b);

                    imageData.AppendLine($"{ir} {ig} {ib}");
                }
            }
            PNGExport export = new PNGExport(imageWidth, imageHeight, "rendernew");
            export.ExportFile(imageData);
        }
    }
}