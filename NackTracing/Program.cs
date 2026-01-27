using System.Text;
using System.IO;

namespace NackTracing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = Path.Combine(userProfilePath, "Downloads");
            Directory.CreateDirectory(downloadsPath);
            string filePath = Path.Combine(downloadsPath, "render.ppm");

            int imageWidth = 256;
            int imageHeight = 256;

            using StreamWriter writer = new StreamWriter(filePath);

            writer.WriteLine("P3");
            writer.WriteLine($"{imageWidth} {imageHeight}");
            writer.WriteLine("255");

            for (int y = 0; y < imageHeight; y++) {
                Console.Title = $"Processing line: {y} of {imageHeight}";
                for (int x = 0; x < imageWidth; x++) { 
                    var r = (double)x / (imageWidth - 1);
                    var g = (double)y / (imageHeight - 1);
                    var b = 0.0;

                    int ir = (int)(255.999 * r);
                    int ig = (int)(255.999 * g);
                    int ib = (int)(255.999 * b);

                    writer.WriteLine($"{ir} {ig} {ib}");
                }
            }
            Console.WriteLine("Render completado, archivo 'render.ppm' creado");
        }
    }
}