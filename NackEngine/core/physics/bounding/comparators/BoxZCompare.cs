using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.physics.bounding.comparators
{
    public class BoxZCompare : IComparer<Hittable>
    {
        public int Compare(Hittable a, Hittable b)
        {
            return a.BoundingBox().Axis(2).Min().CompareTo(b.BoundingBox().Axis(2).Min());
        }
    }
}
