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

        public static NVector Refract(NVector uv, NVector n, float eta)
        {
            var cosine = MathF.Min(NVector.Dot(-uv, n), 1f);
            NVector rayPerpendicular = eta * (uv + cosine * n);
            NVector rayParallel = -MathF.Sqrt(MathF.Abs(1f - rayPerpendicular.LengthSquared())) * n;
            return rayPerpendicular + rayParallel;
        }
    }
}
