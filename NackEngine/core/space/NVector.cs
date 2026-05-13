using NackEngine.math;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace NackEngine.core.space
{
    public struct NVector
    {
        public Vector128<float> V;

        public enum Axis { X, Y, Z }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NVector(float x, float y, float z)
        {
            V = Vector128.Create(x, y, z, 0.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private NVector(Vector128<float> v)
        {
            V = v;
        }

        public float X() => V.GetElement(0);

        public float Y() => V.GetElement(1);

        public float Z() => V.GetElement(2);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared() => Dot(this, this);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length() => MathF.Sqrt(LengthSquared());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator +(NVector a, NVector b) => new NVector(a.V + b.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator -(NVector a, NVector b) => new NVector(a.V - b.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator -(NVector a) => new NVector(-a.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(NVector a, float scalar)
            => new NVector(a.V * Vector128.Create(scalar));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(float scalar, NVector a) => a * scalar;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator *(NVector a, NVector b) => new NVector(a.V * b.V);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NVector operator /(NVector a, float scalar)
             => new NVector(a.V / Vector128.Create(scalar));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(NVector a, NVector b)
        {
            return Vector128.Dot(a.V, b.V);
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

        public bool IsNaN()
        {
            return float.IsNaN(X()) || float.IsNaN(Y()) || float.IsNaN(Z());
        }
    }
}
