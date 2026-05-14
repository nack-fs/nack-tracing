using NackEngine.core.physics;
using NackEngine.math;

namespace NackEngine.core.render.materials
{
    public struct ScatterStruct
    {
        public Color Attenuation;
        public ProbDensity ProbDensity;
        public Ray Bounced;
        public bool SkipProb;
    }
}
