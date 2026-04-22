using NackEngine.core.space;

namespace NackEngine.math
{
    public interface ProbDensity
    {
        float Value(NVector direction);
        NVector Generate();
    }
}
