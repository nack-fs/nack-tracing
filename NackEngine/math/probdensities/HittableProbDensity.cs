using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core.physics;

namespace NackEngine.math.probdensities
{
    using static NackEngine.core.space.NVector;
    using Point = NVector;

    public class HittableProbDensity : ProbDensity
    {
        private Hittable wrappedObj;
        private Point origin;

        public HittableProbDensity(Hittable wrappedObj, Point origin) { 
            this.wrappedObj = wrappedObj;
            this.origin = origin;
        }

        public double Value(NVector direction) => wrappedObj.Probability(origin, direction);
        public NVector Generate() => wrappedObj.Random(origin);
    }
}
