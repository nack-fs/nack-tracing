using NackEngine.core.space;
using NackEngine.math;

namespace NackEngine.core.render.textures.noise
{
    using Point = NVector;

    public class Perlin
    {
        private const int PointCount = 256;

        private NVector[] randVector;
        private int[] permX;
        private int[] permY;
        private int[] permZ;

        public Perlin()
        {
            randVector = new NVector[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                randVector[i] = MathSetting.RandomUnitVector();
            }

            permX = GeneratePerm();
            permY = GeneratePerm();
            permZ = GeneratePerm();
        }

        public float Noise(Point p)
        {
            var u = p.X() - MathF.Floor(p.X());
            var v = p.Y() - MathF.Floor(p.Y());
            var w = p.Z() - MathF.Floor(p.Z());

            var i = (int)MathF.Floor(p.X());
            var j = (int)MathF.Floor(p.Y());
            var k = (int)MathF.Floor(p.Z());

            NVector[,,] c = new NVector[2, 2, 2];

            for (int di = 0; di < 2; di++)
                for (int dj = 0; dj < 2; dj++)
                    for (int dk = 0; dk < 2; dk++)
                        c[di, dj, dk] = randVector[
                            permX[(i + di) & 255] ^
                            permY[(j + dj) & 255] ^
                            permZ[(k + dk) & 255]
                        ];

            return PerlinInterp(c, u, v, w);
        }

        private static float PerlinInterp(NVector[,,] c, float u, float v, float w)
        {
            var uu = u * u * (3 - 2 * u);
            var vv = v * v * (3 - 2 * v);
            var ww = w * w * (3 - 2 * w);
            var accum = 0.0f;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        NVector weightV = new NVector(u - i, v - j, w - k);
                        accum += (i * uu + (1 - i) * (1 - uu)) *
                                 (j * vv + (1 - j) * (1 - vv)) *
                                 (k * ww + (1 - k) * (1 - ww)) *
                                 NVector.Dot(c[i, j, k], weightV);
                    }
                }
            }

            return accum;
        }

        public float Turbulence(Point p, int depth)
        {
            var accum = 0.0f;
            var tempP = p;
            var weight = 1.0f;

            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5f;
                tempP *= 2f;
            }

            return MathF.Abs(accum);
        }

        private static int[] GeneratePerm()
        {
            var p = new int[PointCount];
            for (int i = 0; i < PointCount; i++)
                p[i] = i;

            Permute(p, PointCount);
            return p;
        }

        private static void Permute(int[] p, int n)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int target = MathSetting.RandomInteger(0, i);
                (p[i], p[target]) = (p[target], p[i]);
            }
        }
    }
}
