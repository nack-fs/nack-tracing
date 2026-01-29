using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    public interface Hittable
    {
        public bool Hit(Ray ray, double rayTmin, double rayTmax,out HitStruct hit);
    }
}
