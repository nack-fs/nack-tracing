using NackEngine.core.space;

namespace NackEngine.math
{
    public static class MathSetting
    {
        public static float RandomFloat()
        {
            return (float)Random.Shared.NextDouble();
        }

        public static float RandomFloat(float min, float max)
        {
            return min + (max - min) * RandomFloat();
        }

        public static int RandomInteger(int min, int max)
        {
            return Random.Shared.Next(min, max + 1);
        }

        public static NVector RandomUnitVector()
        {
            while (true)
            {
                var p = NVector.Random(-1, 1);
                var len = p.LengthSquared();
                if (1e-160f < len && len <= 1)
                {
                    return p / MathF.Sqrt(len);
                }
            }
        }

        public static NVector RandomOnHemisphere(NVector normal)
        {
            NVector onUnitSphere = RandomUnitVector();
            return (NVector.Dot(onUnitSphere, normal) > 0.0f) ?
                    onUnitSphere : -onUnitSphere;
        }

        public static NVector RandomUnitDisk()
        {
            while (true)
            {
                var p = new NVector(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f), 0f);
                if (p.LengthSquared() < 1f)
                {
                    return p;
                }
            }
        }

        public static NVector RandomCosineDirection()
        {
            var r1 = RandomFloat();
            var r2 = RandomFloat();
            var z = MathF.Sqrt(1f - r2);

            var phi = 2f * MathF.PI * r1;
            var x = MathF.Cos(phi) * MathF.Sqrt(r2);
            var y = MathF.Sin(phi) * MathF.Sqrt(r2);

            return new NVector(x, y, z);
        }
    }
}
