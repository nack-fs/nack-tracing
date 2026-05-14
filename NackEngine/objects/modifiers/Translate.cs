using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.space;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects.modifiers
{
    public class Translate : Hittable
    {
        private Hittable wrappedObj;
        private NVector offset;

        private AABBox aabbox;

        public Translate(Hittable obj, NVector offset)
        {
            this.wrappedObj = obj;
            this.offset = offset;

            AABBox originalBox = obj.BoundingBox();

            this.aabbox = new AABBox(
                new Range(originalBox.X.Min() + offset.X(), originalBox.X.Max() + offset.X()),
                new Range(originalBox.Y.Min() + offset.Y(), originalBox.Y.Max() + offset.Y()),
                new Range(originalBox.Z.Min() + offset.Z(), originalBox.Z.Max() + offset.Z())
            );
        }

        public AABBox BoundingBox() => aabbox;

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            Ray offsetRay = new Ray(
                ray.Origin() - offset,
                ray.Direction(),
                ray.Time()
            );

            if (!wrappedObj.Hit(offsetRay, range, out hit))
            {
                return false;
            }

            hit.Point += offset;

            return true;
        }
    }
}
