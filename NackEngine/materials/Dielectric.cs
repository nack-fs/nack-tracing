using NackEngine.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.materials
{
    public class Dielectric : Material
    {
        private double indexRefraction;

        public Dielectric(double indexRefraction)
        {
            this.indexRefraction = indexRefraction;
        }

        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, out Ray bounced)
        {
            attenuation = new Color(1.0,1.0,1.0);
            double ri = hit.FrontFace ? (1.0 / indexRefraction) : indexRefraction;

            NVector unitDirection = NVector.UnitVector(ray.Direction());
            NVector refracted = NVector.Refract(unitDirection, hit.Normal, ri);

            bounced = new Ray(hit.Point, refracted);
            return true;
        }
    }
}
