using NackEngine.core.space;

namespace NackEngine.core.render.textures
{
    public class ImageTexture : Texture
    {
        private byte[] pixelData;
        private int width;
        private int height;
        private int bytes;

        public ImageTexture(byte[] pixelData, int width, int height, int bytes)
        {
            this.pixelData = pixelData;
            this.width = width;
            this.height = height;
            this.bytes = bytes;
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
