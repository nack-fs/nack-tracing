using NackEngine.core.physics;
using NackEngine.core.render.textures;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.render.materials.emissive
{
    using Point = NVector;

    public class DiffuseLight : Material
    {
        private Texture texture;

        public DiffuseLight(Texture texture) { 
            this.texture = texture;
        }

        public DiffuseLight(Color emit) {
            this.texture = new SolidColor(emit);
        }

        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, out Ray bounced)
        {
            attenuation = default;
            bounced = default;
            return false;
        }

        public Color Emitted(double u, double v, Point point)
        {
            return texture.Value(u,v,point);
        }
    }
}
