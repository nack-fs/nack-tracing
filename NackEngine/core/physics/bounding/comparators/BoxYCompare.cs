using System;
using System.Collections.Generic;
using System.Text;
using Axis = NackEngine.core.space.NVector.Axis;

namespace NackEngine.core.physics.bounding.comparators
{
    public class BoxYCompare : IComparer<Hittable>
    {
        public int Compare(Hittable a, Hittable b)
        {
            return a.BoundingBox().GetRangeAxis(Axis.Y).Min().CompareTo(b.BoundingBox().GetRangeAxis(Axis.Y).Min());
        }
    }
}
