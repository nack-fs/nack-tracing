using NackEngine.math;

namespace NackEngine.core.space
{
    public struct NVector
    {
        double x;
        double y;
        double z;

        public NVector(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double X() { return this.x;}
        public double Y() { return this.y; }
        public double Z() { return this.z; }

        public double LengthSquared() => x * x + y * y + z * z;

        public double Length() => Math.Sqrt(LengthSquared());

        public static NVector operator +(NVector a, NVector b) =>
            new NVector(a.x + b.x, a.y + b.y, a.z + b.z);

        public static NVector operator -(NVector a, NVector b) =>
            new NVector(a.x - b.x, a.y - b.y, a.z - b.z);

        public static NVector operator -(NVector a) =>
            new NVector(-a.x, -a.y, -a.z);

        public static NVector operator *(NVector a, double scalar) =>
            new NVector(a.x * scalar, a.y * scalar, a.z * scalar);

        public static NVector operator *(double scalar, NVector a) =>
            a * scalar;

        public static NVector operator *(NVector a, NVector b) =>
            new NVector(a.x * b.x, a.y * b.y, a.z * b.z);

        public static NVector operator /(NVector a, double scalar) =>
            new NVector(a.x / scalar, a.y / scalar, a.z / scalar);

        public static double Dot(NVector a, NVector b) =>
            a.x * b.x + a.y * b.y + a.z * b.z;

        public static NVector Cross(NVector a, NVector b)
            => new NVector(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );

        public static NVector UnitVector(NVector a) =>
            a / a.Length();

        public static NVector Random()
        {
            return new NVector(MathSetting.RandomDouble(),
                MathSetting.RandomDouble(), MathSetting.RandomDouble());
        }

        public static NVector Random(double min, double max)
        {
            return new NVector(MathSetting.RandomDouble(min, max),
                MathSetting.RandomDouble(min, max), MathSetting.RandomDouble(min, max));
        }
    }
}
