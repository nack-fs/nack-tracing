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
    public class Triangle : Hittable
    {
        private Point v0, v1, v2;
        private AABBox aabbox;
        private Material material;

        private NVector edge1, edge2;
        private NVector normal;
        private double area;

        private NVector uv0, uv1, uv2;
        private NVector n0, n1, n2;

        public Triangle(Point v0, Point v1, Point v2, 
            NVector uv0,NVector uv1, NVector uv2,
            NVector n0, NVector n1, NVector n2,  Material material) { 

            this.v0 = v0; this.v1 = v1; this.v2 = v2;
            this.n0 = n0; this.n1 = n1; this.n2 = n2;

            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;

            this.material = material;

            this.edge1 = v1 - v0;
            this.edge2 = v2 - v0;

            var n = NVector.Cross(edge1, edge2);
            this.normal = NVector.UnitVector(n);
            this.area = n.Length() * 0.5;

            InitializeBoundingBox();
        }

        private void InitializeBoundingBox() {
            Point min = new Point(
                Math.Min(v0.X(), Math.Min(v1.X(), v2.X())),
                Math.Min(v0.Y(), Math.Min(v1.Y(), v2.Y())),
                Math.Min(v0.Z(), Math.Min(v1.Z(), v2.Z()))
            );

            Point max = new Point(
                Math.Max(v0.X(), Math.Max(v1.X(), v2.X())),
                Math.Max(v0.Y(), Math.Max(v1.Y(), v2.Y())),
                Math.Max(v0.Z(), Math.Max(v1.Z(), v2.Z()))
            );

            double padding = 1e-3;
            min = new Point(min.X() - padding, min.Y() - padding, min.Z() - padding);
            max = new Point(max.X() + padding, max.Y() + padding, max.Z() + padding);

            this.aabbox = new AABBox(min, max);
        }

        public AABBox BoundingBox() => aabbox;
        

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            hit = default;

            NVector direction = ray.Direction();
            NVector h = NVector.Cross(direction, edge2);
            double a = NVector.Dot(edge1, h);

            if (Math.Abs(a) < 1e-15)
            {
                return false;
            }

            double f = 1.0 / a;
            NVector s = ray.Origin() - v0;
            double u = f * NVector.Dot(s, h);

            double eps = 1e-8;

            if (u < -eps || u > 1.0 + eps) return false;

            NVector q = NVector.Cross(s, edge1);
            double v = f * NVector.Dot(direction, q);

            if (v < -eps || u + v > 1.0 + eps) return false;

            double t = f * NVector.Dot(edge2, q);

            if (!range.Contains(t)) { 
                return false;
            }

            hit.T = t;
            hit.Point = ray.At(t);
            hit.Material = material;

            double w = 1.0 - u - v;

            NVector smoothNormal = (n0 * w) + (n1 * u) + (n2 * v);


            hit.U = (w * uv0.X()) + (u * uv1.X()) + (v * uv2.X());
            hit.V = (w * uv0.Y()) + (u * uv1.Y()) + (v * uv2.Y());
            hit.setFaceNormal(ray, NVector.UnitVector(smoothNormal));

            return true;
        }
    }
}
