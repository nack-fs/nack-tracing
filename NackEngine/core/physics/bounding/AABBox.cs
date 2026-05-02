using NackEngine.core.space;
using Range = NackEngine.core.space.Range;
using Axis = NackEngine.core.space.NVector.Axis;

namespace NackEngine.core.physics.bounding
{
    using Point = NVector;

    public struct AABBox
    {
        public static AABBox EMPTY = new AABBox(Range.EMPTY, Range.EMPTY, Range.EMPTY);
        public static AABBox UNIVERSE = new AABBox(Range.UNIVERSE, Range.UNIVERSE, Range.UNIVERSE);

        public Range X, Y, Z;

        public AABBox(Range X, Range Y, Range Z)
        {
            this.X = X; this.Y = Y; this.Z = Z;
            MinPadding();
        }

        public AABBox(Point a, Point b)
        {
            this.X = (a.X() <= b.X()) ? new Range(a.X(), b.X()) : new Range(b.X(), a.X());
            this.Y = (a.Y() <= b.Y()) ? new Range(a.Y(), b.Y()) : new Range(b.Y(), a.Y());
            this.Z = (a.Z() <= b.Z()) ? new Range(a.Z(), b.Z()) : new Range(b.Z(), a.Z());
            MinPadding();
        }

        public AABBox(AABBox box1, AABBox box2)
        {
            this.X = new Range(box1.X, box2.X);
            this.Y = new Range(box1.Y, box2.Y);
            this.Z = new Range(box1.Z, box2.Z);
            MinPadding();
        }

        public Range GetRangeAxis(Axis axis) => axis switch
        {
            Axis.X => this.X,
            Axis.Y => this.Y,
            Axis.Z => this.Z,
            _ => throw new ArgumentException()
        };

        public bool Hit(Ray ray, Range rayT)
        {
            Point rayOrigin = ray.Origin();
            NVector rayDirection = ray.Direction();

            double tMin = rayT.Min();
            double tMax = rayT.Max();

            for (int ax = 0; ax < 3; ax++)
            {
                Axis axis = (Axis)ax;
                Range rangeAxis = GetRangeAxis(axis);

                double invD = 1.0 / rayDirection.GetComponent(axis);
                double origin = rayOrigin.GetComponent(axis);

                double t0 = (rangeAxis.Min() - origin) * invD;
                double t1 = (rangeAxis.Max() - origin) * invD;

                if (invD < 0.0)
                {
                    (t0, t1) = (t1, t0);
                }

                if (t0 > tMin) tMin = t0;
                if (t1 < tMax) tMax = t1;

                if (tMax <= tMin)
                {
                    return false;
                }
            }
            return true;
        }

        public double Area() {
            double dx = X.Size();
            double dy = Y.Size();
            double dz = Z.Size();

            if (dx < 0 || dy < 0 || dz < 0) { return 0.0; }

            return 2.0 * (dx * dy + dy * dz + dz * dx);
        }

        public int LongestAxis()
        {
            if (X.Size() < Y.Size())
            {
                return X.Size() > Z.Size() ? 0 : 2;
            }
            else
            {
                return Y.Size() > Z.Size() ? 1 : 2;
            }
        }

        private void MinPadding()
        {
            double d = 1e-4;
            if (X.Size() < d) X = X.Expand(d);
            if (Y.Size() < d) Y = Y.Expand(d);
            if (Z.Size() < d) Z = Z.Expand(d);
        }

        public static AABBox operator +(AABBox aabbox, NVector offset) =>
            new AABBox(aabbox.X + offset.X(), aabbox.Y + offset.Y(), aabbox.Z + offset.Z());

    }
}
