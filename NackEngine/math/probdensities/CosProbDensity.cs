using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.math.probdensities
{
    public class CosProbDensity : ProbDensity
    {
        private Orthonormal orthonormal;

        public CosProbDensity(NVector w) { 
            this.orthonormal = new Orthonormal(w);
        }

        public NVector Generate()
        {
            return orthonormal.Local(MathSetting.RandomCosineDirection());
        }

        public double Value(NVector direction)
        {
            var cos = NVector.Dot(NVector.UnitVector(direction), orthonormal.W);
            return (cos <= 0) ? 0 : cos / Math.PI;
        }
    }
}
