using NackEngine.core.space;
using NackEngine.math;

namespace NackEngine.core.render.textures.noise
{
    using Point = NVector;

    public class Perlin
    {
        private const int PointCount = 256;

        private double[] randFloat;
        private int[] permX;
        private int[] permY;
        private int[] permZ;

        public Perlin()
        {
            randFloat = new double[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                randFloat[i] = MathSetting.RandomDouble();
            }

            permX = GeneratePerm();
            permY = GeneratePerm();
            permZ = GeneratePerm();
        }

        public double Noise(Point p)
        {
            var i = ((int)Math.Floor(4 * p.X())) & 255;
            var j = ((int)Math.Floor(4 * p.Y())) & 255;
            var k = ((int)Math.Floor(4 * p.Z())) & 255;

            return randFloat[permX[i] ^ permY[j] ^ permZ[k]];
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
