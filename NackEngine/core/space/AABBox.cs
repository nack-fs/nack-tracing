using NackEngine.core.physics;

namespace NackEngine.core.space
{
    using Point = NVector;

    public struct AABBox
    {
        public Range X, Y, Z;

        public AABBox() { }

        public AABBox(Range X, Range Y, Range Z)
        {
            this.X = X; this.Y = Y; this.Z = Z;
        }

        public AABBox(Point a, Point b)
        {
            this.X = (a.X() <= b.X()) ? new Range(a.X(), b.X()) : new Range(b.X(), a.X());
            this.Y = (a.Y() <= b.Y()) ? new Range(a.Y(), b.Y()) : new Range(b.Y(), a.Y());
            this.Z = (a.Z() <= b.Z()) ? new Range(a.Z(), b.Z()) : new Range(b.Z(), a.Z());
        }

        public Range Axis(int n)
        {
            return (n == 1) ? Y : ((n == 2) ? Z : X);
        }

        public bool Hit(Ray ray, Range rayT)
        {
            Point rayOrigin = ray.Origin();
            NVector rayDirection = ray.Direction();

            double tMin = rayT.Min();
            double tMax = rayT.Max();

            for (int ax = 0; ax < 3; ax++)
            {
                Range axis = Axis(ax);
                double invD = 1.0 / rayDirection.getComponent(ax);
                double origin = rayOrigin.getComponent(ax);

                double t0 = (axis.Min() - origin) * invD;
                double t1 = (axis.Max() - origin) * invD;

                if (invD < 0.0f) {
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

    }
}
