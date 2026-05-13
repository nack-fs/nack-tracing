using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NackEngine.core.render.textures
{
    public class FloatImageTexture : Texture
    {
        private float[] data;
        private int width;
        private int height;

        private readonly float exposure;

        public FloatImageTexture(float[] data, int width, int height, float exposure = 1.0f)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.exposure = exposure;
        }

        public Color Value(float u, float v, NVector point)
        {
            if (data == null) { return Color.PINK_HOT; }

            u = Math.Clamp(u, 0.0f, 1.0f);
            v = 1.0f - Math.Clamp(v, 0.0f, 1.0f);

            int x = (int)(u * width);
            int y = (int)(v * height);

            if (x >= width) { x = width - 1; }
            if (y >= height) { y = height - 1; }

            int index = (y * width + x) * 3;

            return new Color(data[index] * exposure, data[index + 1] * exposure, data[index + 2] * exposure);
        }
    }
}
