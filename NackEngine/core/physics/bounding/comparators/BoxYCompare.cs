using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.physics.bounding.comparators
{
    public class BoxYCompare : IComparer<Hittable>
    {
        public int Compare(Hittable a, Hittable b)
        {
            return a.BoundingBox().Axis(1).Min().CompareTo(b.BoundingBox().Axis(1).Min());
        }
    }
}
