using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.math.probdensities
{
    public class SphereProbDensity : ProbDensity
    {
        public NVector Generate()
        {
            return MathSetting.RandomUnitVector();
        }

        public double Value(NVector direction)
        {
            return 1.0 / (4.0 * MathF.PI);
        }
    }
}
