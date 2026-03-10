using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.math.probdensities
{
    public class MixProbDensity : ProbDensity
    {
        private ProbDensity p0;
        private ProbDensity p1;

        public MixProbDensity(ProbDensity p0, ProbDensity p1) { 
            this.p0 = p0; this.p1 = p1;
        }

        public double Value(NVector direction)
        {
            return 0.5 * p0.Value(direction) + 0.5 * p1.Value(direction);
        }

        public NVector Generate()
        {
            return (MathSetting.RandomDouble() < 0.5)?
                p0.Generate() : p1.Generate();
        }
    }
}
