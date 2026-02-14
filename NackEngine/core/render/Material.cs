using NackEngine.core.physics;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.render
{
    public interface Material
    {
        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, 
            out Ray bounced);
    }
}
