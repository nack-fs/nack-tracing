using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.render.textures
{
    public class TestTexture : Texture
    {
        private double inverseScale;
        private Texture tex1;
        private Texture tex2;

        public TestTexture(double scale, Texture tex1, Texture tex2) {
            this.inverseScale = 1.0 / scale;
            this.tex1 = tex1;
            this.tex2 = tex2;
        }

        public TestTexture(double scale, Color color1, Color color2)
                : this(scale, new SolidColor(color1), new SolidColor(color2)) { }

        public Color Value(double u, double v, NVector point)
        {
            var x = Math.Floor(inverseScale * point.X());
            var y = Math.Floor(inverseScale * point.Y());
            var z = Math.Floor(inverseScale * point.Z());

            bool even = (x + y + z) % 2 == 0;
            return even ? tex1.Value(u, v, point) : tex2.Value(u, v, point);
        }
    }
}
