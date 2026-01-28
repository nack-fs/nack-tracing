using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ExportConfig
{
    public class PNGExport
    {
        private readonly int imageWidth;
        private readonly int imageHeight;
        private readonly string fileName;

        public PNGExport(int imageWidth, int imageHeight, string fileName)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.fileName = fileName;
        }

        public void ExportFile(StringBuilder data)
        {

            using var bitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
            using (var reader = new StringReader(data.ToString()))
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++)
                    {
                        string? line = reader.ReadLine();
                        if (line == null) { break; }

                        // Separación "R G B"
                        string[] rgbValues = line.Split(' ',
                            StringSplitOptions.RemoveEmptyEntries);

                        if (rgbValues.Length == 3)
                        {
                            byte r = byte.Parse(rgbValues[0]);
                            byte g = byte.Parse(rgbValues[1]);
                            byte b = byte.Parse(rgbValues[2]);

                            Color pixelColor = Color.FromArgb(r, g, b);
                            bitmap.SetPixel(x, y, pixelColor);
                        }
                    }
                }
            }

            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = Path.Combine(userProfilePath, "Downloads");
            Directory.CreateDirectory(downloadsPath);
            string filePath = Path.Combine(downloadsPath, $"{fileName}.png");

            bitmap.Save(filePath, ImageFormat.Png);
            Console.WriteLine($"Render completado, archivo '{fileName}.png' creado");
        }
    }
}
