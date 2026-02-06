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
            double cosine = Math.Min(NVector.Dot(-unitDirection, hit.Normal), 1.0);
            double sine = Math.Sqrt(1.0 - cosine*cosine);

            bool canRefract = ri * sine <= 1.0;
            NVector direction;

            if (canRefract)
            {
                direction = NVector.Refract(unitDirection, hit.Normal, ri);
            }
            else {
                direction = NVector.Reflect(unitDirection, hit.Normal);
            }
            bounced = new Ray(hit.Point, direction);
            return true;
        }
    }
}
