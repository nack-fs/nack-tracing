using NackEngine.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.objects
{
    public class Collection : Hittable
    {
        private List<Hittable> objects;

        public Collection() {
            this.objects = new List<Hittable>();
        }

        public void addObject(Hittable obj) {
            if (!objects.Contains(obj)) { 
                objects.Add(obj);
            }
        }

        public void removeObject(Hittable obj) {
            objects.Remove(obj);
        }

        public void clear() { 
            objects.Clear();
        }

        public bool Hit(Ray ray, double rayTmin, double rayTmax,out HitStruct hit)
        {
            HitStruct tmpHit = new HitStruct();
            bool hitAnything = false;
            var closestSoFar = rayTmax;
            hit = default;

            foreach(Hittable obj in objects) {
                if (obj.Hit(ray, rayTmin, closestSoFar, out tmpHit)) {
                    hitAnything = true;
                    closestSoFar = tmpHit.t;
                    hit = tmpHit;
                }
            }
            return hitAnything;
        }
    }
}
