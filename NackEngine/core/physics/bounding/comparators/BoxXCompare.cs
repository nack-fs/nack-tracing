using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.physics.bounding.comparators
{
    public class BoxXCompare : IComparer<Hittable>
    {
        public int Compare(Hittable a, Hittable b) => CompareAxis(a, b, 0);
        private int CompareAxis(Hittable a, Hittable b, int axisIndex)
        {
            double minA = a.BoundingBox().Axis(axisIndex).Min();
            double minB = b.BoundingBox().Axis(axisIndex).Min();
            return minA.CompareTo(minB);
        }
    }
}
