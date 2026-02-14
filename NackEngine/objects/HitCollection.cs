using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects
{
    public class HitCollection : Hittable
    {
        private List<Hittable> objects;
        private AABBox aabbox;
        private bool initialized;

        public HitCollection()
        {
            this.objects = new List<Hittable>();
        }

        public void AddObject(Hittable obj)
        {
            objects.Add(obj);

            if (!initialized)
            {
                aabbox = obj.BoundingBox();
                initialized = true;
            }
            else
            {
                aabbox = new AABBox(aabbox, obj.BoundingBox());
            }
        }

        public void RemoveObject(Hittable obj)
        {
            objects.Remove(obj);
        }

        public void Clear()
        {
            objects.Clear();
        }

        public bool Hit(Ray ray, Range rayT, out HitStruct hit)
        {
            HitStruct tmpHit = new HitStruct();
            bool hitAnything = false;
            var closestSoFar = rayT.Max();
            hit = default;

            foreach (Hittable obj in objects)
            {
                if (obj.Hit(ray, new Range(rayT.Min(), closestSoFar), out tmpHit))
                {
                    hitAnything = true;
                    closestSoFar = tmpHit.T;
                    hit = tmpHit;
                }
            }
            return hitAnything;
        }

        public AABBox BoundingBox()
        {
            return aabbox;
        }

        public List<Hittable> GetObjects() { return objects; }
    }
}
