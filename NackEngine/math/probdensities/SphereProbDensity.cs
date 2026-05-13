using NackEngine.core.space;

namespace NackEngine.math.probdensities
{
    public class SphereProbDensity : ProbDensity
    {
        public NVector Generate()
        {
            return MathSetting.RandomUnitVector();
        }

        public float Value(NVector direction)
        {
            return 1.0f / (4.0f * MathF.PI);
        }
    }
}
