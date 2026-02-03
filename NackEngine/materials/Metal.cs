using NackEngine.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.materials
{
    public class Metal : Material
    {
        private Color albedo;

        public Metal(Color albedo)
        {
            this.albedo = albedo;
        }

        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, out Ray bounced)
        {
            NVector reflected = NVector.Reflect(ray.Direction(), hit.Normal);
            bounced = new Ray(hit.Point, reflected);
            attenuation = albedo;
            return true; 
        }
    }
}
