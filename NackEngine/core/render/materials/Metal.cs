using NackEngine.core.physics;
using NackEngine.core.render;
using NackEngine.core.space;
using NackEngine.math;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.render.materials
{
    public class Metal : Material
    {
        private Color albedo;
        private float fuzz;

        public Metal(Color albedo, float fuzz)
        {
            this.albedo = albedo;
            this.fuzz = MathF.Min(fuzz, 1f);
        }

        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter)
        {
            scatter = new ScatterStruct();
            scatter.SkipProb = true;
            scatter.ProbDensity = null;

            scatter.Attenuation = albedo;

            NVector reflected = RayPhysics.Reflect(ray.Direction(), hit.Normal);
            reflected = NVector.UnitVector(reflected) + (fuzz * MathSetting.RandomUnitVector());
            scatter.Bounced = new Ray(hit.Point, reflected, ray.Time());
            
            return (NVector.Dot(scatter.Bounced.Direction(), hit.Normal)>0); 
        }
    }
}
