using NackEngine.core.physics;
using NackEngine.core.space;

namespace NackEngine.math.probdensities
{
    using Point = NVector;

    public class HittableProbDensity : ProbDensity
    {
        private Hittable wrappedObj;
        private Point origin;

        public HittableProbDensity(Hittable wrappedObj, Point origin)
        {
            this.wrappedObj = wrappedObj;
            this.origin = origin;
        }

        public float Value(NVector direction) => wrappedObj.Probability(origin, direction);
        public NVector Generate() => wrappedObj.Random(origin);
    }
}
