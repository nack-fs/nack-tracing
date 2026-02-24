using System;
using System.Collections.Generic;
using System.Text;
using Axis = NackEngine.core.space.NVector.Axis;

namespace NackEngine.core.physics.bounding.comparators
{
    public class BoxZCompare : IComparer<Hittable>
    {
        public int Compare(Hittable a, Hittable b)
        {
            return a.BoundingBox().GetRangeAxis(Axis.Z).Min().CompareTo(b.BoundingBox().GetRangeAxis(Axis.Z).Min());
        }
    }
}
