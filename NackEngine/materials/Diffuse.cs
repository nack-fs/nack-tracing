using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core.physics;
using NackEngine.core.render;
using NackEngine.core.space;

namespace NackEngine.materials
{
    public class Diffuse : Material
    {
        private Color albedo;

        public Diffuse(Color albedo)
        {
            this.albedo = albedo;
        }

        public bool Bounce(Ray ray, HitStruct hit,out Color attenuation,out Ray bounced)
        {
            var bounceDirection = hit.Normal + NVector.RandomUnitVector();
            if (bounceDirection.LengthSquared() < 1e-8) {
                bounceDirection = hit.Normal;
            }
            bounced = new Ray(hit.Point, bounceDirection);
            attenuation = this.albedo;
            return true;
        }
    }
}
