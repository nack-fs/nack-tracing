using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects
{
    using Point = NVector;

    public class Plane : Hittable
    {
        private Point corner;
        private NVector u, v;
        private Material material;
        private AABBox aabbox;

        private NVector normal;
        private double D;
        private NVector w;

        public Plane(Point corner, NVector u, NVector v, Material material) { 
            this.corner = corner;
            this.u = u;
            this.v = v;
            this.material = material;

            var n = NVector.Cross(u, v);
            this.normal = NVector.UnitVector(n);
            this.D = NVector.Dot(normal, corner);
            this.w = n / NVector.Dot(n,n);

            InitializeBoundingBox();
        }

        private void InitializeBoundingBox() {
            var diagonal1 = new AABBox(corner, corner + u + v);
            var diagonal2 = new AABBox(corner + u, corner + v);
            this.aabbox = new AABBox(diagonal1, diagonal2);
        }

        public AABBox BoundingBox()
        {
            return aabbox;
        }

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            hit = default;
            var dNormal = NVector.Dot(normal, ray.Direction());

            double tol = 1e-8;
            if (Math.Abs(dNormal) < tol) {
                return false;
            }

            var t = (D - NVector.Dot(normal, ray.Origin())) / dNormal;
            if (!range.Contains(t)) { return false; }

            var intersect = ray.At(t);
            NVector hitVector = intersect - corner;
            var a = NVector.Dot(w, NVector.Cross(hitVector, v));
            var b = NVector.Dot(w, NVector.Cross(u, hitVector));

            if (!IsInside(a, b, out hit)) { return false; }

            hit.T = t;
            hit.Point = intersect;
            hit.Material = material;
            hit.setFaceNormal(ray, normal);

            return true;
        }

        private bool IsInside(double a, double b, out HitStruct hit) {
            hit = default;
            Range unitRange = new Range(0, 1);

            if (!unitRange.Contains(a) || !unitRange.Contains(b)) { 
                return false;
            }

            hit.U = a;
            hit.V = b;
            return true;
        }
    }
}
