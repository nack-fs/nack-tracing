using NackEngine.core.space;

namespace NackEngine.math
{
    public interface ProbDensity
    {
        double Value(NVector direction);
        NVector Generate();
    }
}
