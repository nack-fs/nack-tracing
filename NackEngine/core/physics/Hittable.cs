using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.core.physics
{
    public interface Hittable
    {
        public bool Hit(Ray ray, Range range,out HitStruct hit);
    }
}
