using NackEngine.math;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace NackEngine.core.space
{
    public struct NVector
    {
        private float x, y, z;

        public enum Axis { X, Y, Z}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NVector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float X() => x;

        public float Y() => y;

        public float Z() => z;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared() => Dot(this, this);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length() => MathF.Sqrt(LengthSquared());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator +(NVector a, NVector b) =>
            new NVector(a.x + b.x, a.y + b.y, a.z + b.z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator -(NVector a, NVector b) =>
            new NVector(a.x - b.x, a.y - b.y, a.z - b.z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator -(NVector a) =>
            new NVector(-a.x, -a.y, -a.z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(NVector a, float scalar)
            => new NVector(a.x * scalar, a.y * scalar, a.z * scalar);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(float scalar, NVector a) => a * scalar;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(NVector a, NVector b) =>
            new NVector(a.x * b.x, a.y * b.y, a.z * b.z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator /(NVector a, float scalar)
             => new NVector(a.x / scalar, a.y / scalar, a.z / scalar);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(NVector a, NVector b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
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
            return new NVector(MathSetting.RandomFloat(), MathSetting.RandomFloat(), MathSetting.RandomFloat());
        }

        public static NVector Random(float min, float max)
        {
            return new NVector(MathSetting.RandomFloat(min, max),
                MathSetting.RandomFloat(min, max), MathSetting.RandomFloat(min, max));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetComponent(Axis axis) => axis switch
        {
            Axis.X => X(),
            Axis.Y => Y(),
            Axis.Z => Z(),
            _ => throw new IndexOutOfRangeException()
        };

        public NVector Rotate(float sinTheta, float cosTheta, Axis axis) => axis switch
        {
            Axis.X => new NVector(X(), cosTheta * Y() - sinTheta * Z(), sinTheta * Y() + cosTheta * Z()),
            Axis.Y => new NVector(cosTheta * X() + sinTheta * Z(), Y(), -sinTheta * X() + cosTheta * Z()),
            Axis.Z => new NVector(cosTheta * X() - sinTheta * Y(), sinTheta * X() + cosTheta * Y(), Z()),
            _ => throw new IndexOutOfRangeException()
        };

        public bool IsNaN() {
            return float.IsNaN(X()) || float.IsNaN(Y()) || float.IsNaN(Z());
        }
    }
}
