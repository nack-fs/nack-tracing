using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    public interface Material
    {
        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, 
            out Ray bounced);
    }
}
