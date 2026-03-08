using NackEngine.core.physics;
using NackEngine.math;
using System;
using System.Collections.Generic;
using System.Text;

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
