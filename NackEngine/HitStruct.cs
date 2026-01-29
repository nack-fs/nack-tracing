using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine
{
    using Point = NackEngine.NVector;
    public struct HitStruct
    {
        public Point point { set; get; }
        public NVector normal { set; get; }
        double t { set; get; }
    }
}
