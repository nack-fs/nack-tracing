using NackEngine.core.physics;
using System;
using System.Collections.Generic;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects
{
    public class HitCollection : Hittable
    {
        private List<Hittable> objects;

        public HitCollection() {
            this.objects = new List<Hittable>();
        }

        public void AddObject(Hittable obj) {
            if (!objects.Contains(obj)) { 
                objects.Add(obj);
            }
        }

        public void RemoveObject(Hittable obj) {
            objects.Remove(obj);
        }

        public void Clear() { 
            objects.Clear();
        }

        public bool Hit(Ray ray, Range rayT,out HitStruct hit)
        {
            HitStruct tmpHit = new HitStruct();
            bool hitAnything = false;
            var closestSoFar = rayT.Max();
            hit = default;

            foreach(Hittable obj in objects) {
                if (obj.Hit(ray, new Range(rayT.Min(),closestSoFar), out tmpHit)) {
                    hitAnything = true;
                    closestSoFar = tmpHit.T;
                    hit = tmpHit;
                }
            }
            return hitAnything;
        }
    }
}
