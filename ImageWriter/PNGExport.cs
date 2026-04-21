using NackEngine.IO;
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

                        double r = (col.Vector().X() > 0) ? Math.Sqrt(col.Vector().X()) : 0;
                        double g = (col.Vector().Y() > 0) ? Math.Sqrt(col.Vector().Y()) : 0;
                        double b = (col.Vector().Z() > 0) ? Math.Sqrt(col.Vector().Z()) : 0;

                        int ir = (int)(256 * Math.Clamp(r, 0.0, 0.999));
                        int ig = (int)(256 * Math.Clamp(g, 0.0, 0.999));
                        int ib = (int)(256 * Math.Clamp(b, 0.0, 0.999));

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
            Logger.Log($"[INFO] Rendering complete, file ‘{fileName}.png’ saved successfully.");
        }
    }
}
