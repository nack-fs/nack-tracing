using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.render.textures
{
    public class SolidColor : Texture
    {
        private Color albedo;

        public SolidColor(Color albedo) {
            this.albedo = albedo;
        }

        public SolidColor(double R, double G, double B) {
            this.albedo = new Color(R,G,B);
        }

        public Color Value(double u, double v, NVector point)
        {
            return albedo;
        }
    }
}
