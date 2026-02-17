using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core.physics;
using NackEngine.core.render;
using NackEngine.core.render.textures;
using NackEngine.core.space;
using NackEngine.math;

namespace NackEngine.core.render.materials
{
    public class Diffuse : Material
    {
        private Texture texture;

        public Diffuse(Texture texture)
        {
            this.texture = texture;
        }

        public Diffuse(Color albedo) : this(new SolidColor(albedo)) { }

        public bool Bounce(Ray ray, HitStruct hit,out Color attenuation,out Ray bounced)
        {
            var bounceDirection = hit.Normal + MathSetting.RandomUnitVector();
            if (bounceDirection.LengthSquared() < 1e-8) {
                bounceDirection = hit.Normal;
            }
            bounced = new Ray(hit.Point, bounceDirection, ray.Time());
            attenuation = this.texture.Value(hit.U, hit.V, hit.Point);
            return true;
        }
    }
}
