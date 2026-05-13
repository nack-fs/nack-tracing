using NackEngine.core.physics;
using NackEngine.core.render;
using NackEngine.core.space;
using NackEngine.math;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.render.materials
{
    public class Dielectric : Material
    {
        private float indexRefraction;
        private Color albedo;

        public Dielectric(float indexRefraction, Color? albedo = null)
        {
            this.indexRefraction = indexRefraction;
            this.albedo = albedo ?? Color.WHITE;
        }

        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter)
        {
            scatter = new ScatterStruct();
            scatter.Attenuation = albedo;
            scatter.SkipProb = true;
            scatter.ProbDensity = null;

            float ri = hit.FrontFace ? (1.0f / indexRefraction) : indexRefraction;

            NVector unitDirection = NVector.UnitVector(ray.Direction());
            float cos = MathF.Min(NVector.Dot(-unitDirection, hit.Normal), 1.0f);
            float sin = MathF.Sqrt(1.0f - cos*cos);

            bool canRefract = ri * sin <= 1.0f;
            NVector direction;

            if (canRefract || Reflectance(cos, ri) <= MathSetting.RandomFloat())
            {
                direction = RayPhysics.Refract(unitDirection, hit.Normal, ri);
            }
            else {
                direction = RayPhysics.Reflect(unitDirection, hit.Normal);
            }
            scatter.Bounced = new Ray(hit.Point, direction, ray.Time());
            return true;
        }

        private static float Reflectance(float cosine, float indexRefraction) {
            var r0 = (1f - indexRefraction) / (1f + indexRefraction);
            r0 *= r0;
            return r0 + (1f - r0) * MathF.Pow((1f - cosine), 5f);
        }
    }
}
