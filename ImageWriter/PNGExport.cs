using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using NackColor = NackEngine.core.render.Color;

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

        public void ExportFile(NackColor[] pixelColors)
        {

            using var bitmap = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++)
                    {
                        var col = pixelColors[y * imageWidth + x];

                        float r = col.Vector().X();
                        float g = col.Vector().Y();
                        float b = col.Vector().Z();

                        r = r / (r + 1.0f);
                        g = g / (g + 1.0f);
                        b = b / (b + 1.0f);

                        r = (r > 0f) ? MathF.Sqrt(r) : 0f;
                        g = (g > 0f) ? MathF.Sqrt(g) : 0f;
                        b = (b > 0f) ? MathF.Sqrt(b) : 0f;

                        int ir = (int)(256 * Math.Clamp(r, 0.0f, 0.999f));
                        int ig = (int)(256 * Math.Clamp(g, 0.0f, 0.999f));
                        int ib = (int)(256 * Math.Clamp(b, 0.0f, 0.999f));


                        Color pixelColor = Color.FromArgb(ir, ig, ib);
                        bitmap.SetPixel(x, y, pixelColor);
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
