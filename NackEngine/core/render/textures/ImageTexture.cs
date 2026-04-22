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

        public Color Value(float u, float v, NVector point)
        {
            if (pixelData == null) { return new Color(1, 0, 1); }

            u = Math.Clamp(u, 0.0f, 1.0f);
            v = 1.0f - Math.Clamp(v, 0.0f, 1.0f);

            int i = (int)(u * width);
            int j = (int)(v * height);

            if (i >= width) i = width - 1;
            if (j >= height) j = height - 1;

            int index = (j * bytes) + (i * 3);

            byte b = pixelData[index];
            byte g = pixelData[index + 1];
            byte r = pixelData[index + 2];

            float scale = 1.0f / 255.0f;
            return new Color(r * scale, g * scale, b * scale);
        }
    }
}
