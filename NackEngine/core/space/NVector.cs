using NackEngine.math;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace NackEngine.core.space
{
    public struct NVector
    {
        public Vector256<double> V;

        public enum Axis { X, Y, Z}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NVector(double x, double y, double z)
        {
            V = Vector256.Create(x, y, z, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NVector(Vector256<double> v)
        {
            V = v;
        }

        public double X() => V.GetElement(0);

        public double Y() => V.GetElement(1);

        public double Z() => V.GetElement(2);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double LengthSquared() => Dot(this, this);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length() => Math.Sqrt(LengthSquared());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator +(NVector a, NVector b) => new NVector(a.V + b.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator -(NVector a, NVector b) => new NVector(a.V - b.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator -(NVector a) => new NVector(-a.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(NVector a, double scalar)
            => new NVector(a.V * Vector256.Create(scalar));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(double scalar, NVector a) => a * scalar;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(NVector a, NVector b) => new NVector(a.V * b.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator /(NVector a, double scalar)
             => new NVector(a.V / Vector256.Create(scalar));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(NVector a, NVector b)
        {
            return Vector256.Dot(a.V, b.V);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector Cross(NVector a, NVector b)
        {
            return new NVector(
                a.Y() * b.Z() - a.Z() * b.Y(),
                a.Z() * b.X() - a.X() * b.Z(),
                a.X() * b.Y() - a.Y() * b.X()
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector UnitVector(NVector a) => a / a.Length();

        public static NVector Random()
        {
            return new NVector(MathSetting.RandomDouble(), MathSetting.RandomDouble(), MathSetting.RandomDouble());
        }

        public static NVector Random(double min, double max)
        {
            return new NVector(MathSetting.RandomDouble(min, max),
                MathSetting.RandomDouble(min, max), MathSetting.RandomDouble(min, max));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetComponent(Axis axis) => axis switch
        {
            Axis.X => X(),
            Axis.Y => Y(),
            Axis.Z => Z(),
            _ => throw new IndexOutOfRangeException()
        };

        public NVector Rotate(double sinTheta, double cosTheta, Axis axis) => axis switch
        {
            Axis.X => new NVector(X(), cosTheta * Y() - sinTheta * Z(), sinTheta * Y() + cosTheta * Z()),
            Axis.Y => new NVector(cosTheta * X() + sinTheta * Z(), Y(), -sinTheta * X() + cosTheta * Z()),
            Axis.Z => new NVector(cosTheta * X() - sinTheta * Y(), sinTheta * X() + cosTheta * Y(), Z()),
            _ => throw new IndexOutOfRangeException()
        };

        public bool IsNaN() {
            return double.IsNaN(X()) || double.IsNaN(Y()) || double.IsNaN(Z());
        }
    }
}
