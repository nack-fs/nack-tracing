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

        public SolidColor(float R, float G, float B) {
            this.albedo = new Color(R,G,B);
        }

        public Color Value(float u, float v, NVector point)
        {
            return albedo;
        }
    }
}
