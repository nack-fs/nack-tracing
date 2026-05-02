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

        private readonly double exp = 10;

        public FloatImageTexture(float[] data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }

        public Color Value(double u, double v, NVector point)
        {
            if (data == null) { return Color.PINK_HOT; }

            u = Math.Clamp(u, 0.0, 1.0);
            v = 1.0 - Math.Clamp(v, 0.0, 1.0);

            int x = (int)(u * width);
            int y = (int)(v * height);

            if (x >= width) { x = width - 1; }
            if (y >= height) { y = height - 1; }

            int index = (y * width + x) * 3;

            return new Color(data[index] * exp, data[index + 1] * exp, data[index + 2] * exp);
        }
    }
}
