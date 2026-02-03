using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core;
using Range = NackEngine.core.Range;


namespace NackEngine.objects
{
    using Point = NVector;
    public class Sphere : Hittable
    {
        private Point center;
        private double radius;
        private Material material;

        public Sphere(Point center, double radius, Material material)
        {
            this.center = center;
            this.radius = Math.Max(0, radius);
            this.material = material;
        }

        public bool Hit(Ray ray, Range rayT,out HitStruct hit)
        {
            NVector oc = center - ray.Origin();
            var a = ray.Direction().LengthSquared();
            var h = NVector.Dot(ray.Direction(), oc);
            var c = oc.LengthSquared() - radius * radius;
            hit = default;

            var discriminant = h * h - a * c;
            if (discriminant < 0) {
                return false;
            }

            var sqrtDis = Math.Sqrt(discriminant);

            // Find the nearest root
            var root = (h - sqrtDis) / a;
            if (!rayT.Surrounds(root)) {
                root = (h + sqrtDis) / a;
                if (!rayT.Surrounds(root)) {
                    return false;
                }
            }

            hit.T = root;
            hit.Point = ray.At(hit.T);
            NVector owNormal = (hit.Point - center) / radius;
            hit.setFaceNormal(ray, owNormal);
            hit.Material = material;

            return true;
        }
    }
}
