using NackEngine.core.physics;
using NackEngine.core.render;
using NackEngine.core.space;
using NackEngine.math;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.materials
{
    public class Metal : Material
    {
        private Color albedo;
        private double fuzz;

        public Metal(Color albedo, double fuzz)
        {
            this.albedo = albedo;
            this.fuzz = Math.Min(fuzz, 1);
        }

        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, out Ray bounced)
        {
            NVector reflected = RayPhysics.Reflect(ray.Direction(), hit.Normal);
            reflected = NVector.UnitVector(reflected) + (fuzz * MathSetting.RandomUnitVector());
            bounced = new Ray(hit.Point, reflected);
            attenuation = albedo;
            return (NVector.Dot(bounced.Direction(), hit.Normal)>0); 
        }
    }
}
