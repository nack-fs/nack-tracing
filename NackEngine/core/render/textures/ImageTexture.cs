using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace NackEngine.core.render.textures
{
    public class ImageTexture : Texture
    {
        private byte[] pixelData;
        private int width;
        private int height;
        private int bytes;


        public ImageTexture(string filename) {
            try
            {
                using (var image = new Bitmap(filename))
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
            catch (Exception) {
                Console.WriteLine($"ERROR: No se pudo cargar la textura: {filename}");
                this.pixelData = null;
            }
        }

        public Color Value(double u, double v, NVector point)
        {
            if (pixelData == null) { return new Color(1,0,1); } // ERROR

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
