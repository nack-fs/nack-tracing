using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core;


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

        public bool Hit(Ray ray, double rayTmin, double rayTmax, HitStruct hit)
        {
            NVector oc = center - ray.Origin();
            var a = ray.Direction().LengthSquared();
            var h = NVector.Dot(ray.Direction(), oc);
            var c = oc.LengthSquared() - radius * radius;

            var discriminant = h * h - a * c;
            if (discriminant < 0) {
                return false;
            }

            var sqrtDis = Math.Sqrt(discriminant);

            // Find the nearest root
            var root = (h - sqrtDis) / a;
            if (root <= rayTmin || rayTmax <= root) {
                root = (h + sqrtDis) / a;
                if (root <= rayTmin || rayTmax <= root) {
                    return false;
                }
            }

            hit.t = root;
            hit.point = ray.At(hit.t);
            hit.normal = (hit.point - center) / radius;

            return true;
        }
    }
}
