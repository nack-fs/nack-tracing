using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.space;
using NackEngine.math;
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
        private float D;
        private NVector w;

        private float area;

        public Plane(Point corner, NVector u, NVector v, Material material)
        {
            this.corner = corner;
            this.u = u;
            this.v = v;
            this.material = material;

            var n = NVector.Cross(u, v);
            this.normal = NVector.UnitVector(n);
            this.D = NVector.Dot(normal, corner);
            this.w = n / NVector.Dot(n, n);

            this.area = NVector.Cross(u, v).Length();

            InitializeBoundingBox();
        }

        private void InitializeBoundingBox()
        {
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

            float tol = 1e-8f;
            if (Math.Abs(dNormal) < tol)
            {
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

        private bool IsInside(float a, float b, out HitStruct hit)
        {
            hit = default;
            Range unitRange = new Range(0f, 1f);

            if (!unitRange.Contains(a) || !unitRange.Contains(b))
            {
                return false;
            }

            hit.U = a;
            hit.V = b;
            return true;
        }

        public float Probability(Point origin, NVector direction)
        {
            HitStruct hit;
            if (!this.Hit(new Ray(origin, direction), Range.DEFAULT, out hit))
            {
                return 0f;
            }

            float distanceSquared = hit.T * hit.T * direction.LengthSquared();
            float cos = MathF.Abs(NVector.Dot(direction, hit.Normal) / direction.Length());

            return distanceSquared / (cos * area);
        }

        public NVector Random(Point origin)
        {
            var p = corner +
                (MathSetting.RandomFloat() * u) +
                (MathSetting.RandomFloat() * v);
            return p - origin;
        }
    }
}
