using NackEngine.core.render.textures;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NackEngine.IO
{
    public static class ImageLoader
    {
        public static ImageTexture Load(string filename)
        {
            string path = TextureConfig.GetImagePath(filename);
            if (string.IsNullOrEmpty(path)) { path = filename; }

            try
            {
                using (var image = new Bitmap(path))
                {
                    int width = image.Width;
                    int height = image.Height;

                    var rectangle = new Rectangle(0, 0, width, height);
                    var bmpData = image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    int bytes = Math.Abs(bmpData.Stride);
                    int totalBytes = bytes * height;
                    byte[] pixelData = new byte[totalBytes];

                    Marshal.Copy(bmpData.Scan0, pixelData, 0, totalBytes);

                    image.UnlockBits(bmpData);

                    return new ImageTexture(pixelData, width, height, bytes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] The texture '{filename}' could not be loaded: {e.Message}");
                return new ImageTexture(null, 0, 0, 0);
            }
        }

    }
}
