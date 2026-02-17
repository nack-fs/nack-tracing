using NackEngine.core.space;
using NackEngine.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NackEngine.core.render.textures
{
    public class ImageTexture : Texture
    {
        private byte[] pixelData;
        private int width;
        private int height;
        private int bytes;


        public ImageTexture(string filename)
        {
            string path = TextureConfig.GetImagePath(filename);
            if (string.IsNullOrEmpty(path)) { path = filename; }

            try
            {
                using (var image = new Bitmap(path))
                {
                    width = image.Width;
                    height = image.Height;

                    var rectangle = new Rectangle(0, 0, width, height);
                    var bmpData = image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    bytes = Math.Abs(bmpData.Stride);
                    int totalBytes = bytes * height;
                    pixelData = new byte[totalBytes];

                    Marshal.Copy(bmpData.Scan0, pixelData, 0, totalBytes);

                    image.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] The texture {filename} could not be loaded... Because {e.Message}");
                this.pixelData = null;
            }
        }

        public Color Value(double u, double v, NVector point)
        {
            if (pixelData == null) { return new Color(1, 0, 1); } // ERROR

            u = Math.Clamp(u, 0.0, 1.0);
            v = 1.0 - Math.Clamp(v, 0.0, 1.0);

            // From UV to pixel
            int i = (int)(u * width);
            int j = (int)(v * height);

            if (i >= width) i = width - 1;
            if (j >= height) j = height - 1;

            int index = (j * bytes) + (i * 3);

            byte b = pixelData[index];
            byte g = pixelData[index + 1];
            byte r = pixelData[index + 2];

            double scale = 1.0 / 255.0;
            return new Color(r * scale, g * scale, b * scale);
        }
    }
}
