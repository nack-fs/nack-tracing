using NackEngine.core.physics.bounding;
using NackEngine.core.space;
using Range = NackEngine.core.space.Range;

namespace NackEngine.core.physics
{
    using Point = NVector;

    public interface Hittable
    {
        public bool Hit(Ray ray, Range range, out HitStruct hit);

        public AABBox BoundingBox();

        public float Probability(Point origin, NVector direction) => 0.0f;

        public NVector Random(Point origin) => new NVector(1.0f, 0.0f, 0.0f);
    }
}
