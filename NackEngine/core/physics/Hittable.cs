using NackEngine.core.physics.bounding;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.core.physics
{
    using static NackEngine.core.space.NVector;
    using Point = NVector;

    public interface Hittable
    {
        public bool Hit(Ray ray, Range range,out HitStruct hit);

        public AABBox BoundingBox();

        public double Probability(Point origin, NVector direction) => 0.0;

        public NVector Random(Point origin) => new NVector(1, 0, 0);
    }
}
