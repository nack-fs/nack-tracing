using System;
using System.Collections.Generic;
using System.Text;
using Axis = NackEngine.core.space.NVector.Axis;

namespace NackEngine.core.physics.bounding.comparators
{
    public class BoxXCompare : IComparer<Hittable>
    {
        public int Compare(Hittable a, Hittable b) => CompareAxis(a, b, 0);
        private int CompareAxis(Hittable a, Hittable b, Axis axis)
        {
            float minA = a.BoundingBox().GetRangeAxis(axis).Min();
            float minB = b.BoundingBox().GetRangeAxis(axis).Min();
            return minA.CompareTo(minB);
        }
    }
}
