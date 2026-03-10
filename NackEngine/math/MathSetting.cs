using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NackEngine.math
{
    public static class MathSetting
    {
        public static double RandomDouble()
        {
            return Random.Shared.NextDouble();
        }

        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * Random.Shared.NextDouble();
        }

        // Random integer in [min, max]
        public static int RandomInteger(int min, int max)
        {
            return (int)RandomDouble(min, max+1);
        }

        public static NVector RandomUnitVector()
        {
            while (true)
            {
                var p = NVector.Random(-1, 1);
                var len = p.LengthSquared();
                if (1e-160 < len && len <= 1)
                {
                    return p / Math.Sqrt(len);
                }
            }
        }

        public static NVector RandomOnHemisphere(NVector normal)
        {
            NVector onUnitSphere = RandomUnitVector();
            return (NVector.Dot(onUnitSphere, normal) > 0.0) ?
                    onUnitSphere : -onUnitSphere;
        }

        public static NVector RandomUnitDisk()
        {
            while (true)
            {
                var p = new NVector(MathSetting.RandomDouble(-1, 1), MathSetting.RandomDouble(-1, 1), 0);
                if (p.LengthSquared() < 1)
                {
                    return p;
                }
            }
        }

        public static NVector RandomCosineDirection()
        {
            var r1 = MathSetting.RandomDouble();
            var r2 = MathSetting.RandomDouble();
            var z = Math.Sqrt(1 - r2);

            var phi = 2 * Math.PI * r1;
            var x = Math.Cos(phi) * Math.Sqrt(r2);
            var y = Math.Sin(phi) * Math.Sqrt(r2);

            return new NVector(x, y, z);
        }
    }
}
