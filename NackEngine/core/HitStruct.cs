using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    using Point = NVector;
    public struct HitStruct
    {
        public Point point { set; get; }
        public NVector normal { set; get; }
        public double t { set; get; }
    }
}
