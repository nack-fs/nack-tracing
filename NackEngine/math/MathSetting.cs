using System;
using System.Collections.Generic;
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
    }
}
