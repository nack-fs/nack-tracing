using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    public interface Hittable
    {
        public bool Hit(Ray ray, Range range,out HitStruct hit);
    }
}
