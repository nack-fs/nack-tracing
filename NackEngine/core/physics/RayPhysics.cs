using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.physics
{
    public static class RayPhysics
    {
        public static NVector Reflect(NVector v, NVector n)
        {
            return v - 2 * NVector.Dot(v, n) * n;
        }

        public static NVector Refract(NVector uv, NVector n, double eta)
        {
            var cosine = Math.Min(NVector.Dot(-uv, n), 1);
            NVector rayPerpendicular = eta * (uv + cosine * n);
            NVector rayParallel = -Math.Sqrt(Math.Abs(1 - rayPerpendicular.LengthSquared())) * n;
            return rayPerpendicular + rayParallel;
        }
    }
}
