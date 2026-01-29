using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine
{
    public interface Hittable
    {
        public bool Hit(Ray ray, double rayTmin, double rayTmax, HitStruct hit);
    }
}
