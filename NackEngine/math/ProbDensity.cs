using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.math
{
    public interface ProbDensity
    {
        float Value(NVector direction);
        NVector Generate();
    }
}
