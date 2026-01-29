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

        public Sphere(Point center, double radius)
        {
            this.center = center;
            this.radius = Math.Max(0, radius);
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

            hit.t = root;
            hit.point = ray.At(hit.t);
            NVector owNormal = (hit.point - center) / radius;
            hit.setFaceNormal(ray, owNormal);

            return true;
        }
    }
}
