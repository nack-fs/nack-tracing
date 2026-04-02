using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects
{
    using Point = NVector;

    public class Box : Hittable
    {
        private HitCollection sides = new HitCollection();

        public Box(Point a, Point b, Material material)
        {
            var min = new Point(
                MathF.Min(a.X(), b.X()),
                MathF.Min(a.Y(), b.Y()),
                MathF.Min(a.Z(), b.Z())
            );

            var max = new Point(
                MathF.Max(a.X(), b.X()),
                MathF.Max(a.Y(), b.Y()),
                MathF.Max(a.Z(), b.Z())
            );

            var dx = new NVector(max.X() - min.X(), 0f, 0f);
            var dy = new NVector(0f, max.Y() - min.Y(), 0f);
            var dz = new NVector(0f, 0f, max.Z() - min.Z());

            sides.AddObject(new Plane(new Point(min.X(), min.Y(), max.Z()), dx, dy, material));
            sides.AddObject(new Plane(new Point(max.X(), min.Y(), max.Z()), -dz, dy, material));
            sides.AddObject(new Plane(new Point(max.X(), min.Y(), min.Z()), -dx, dy, material));
            sides.AddObject(new Plane(new Point(min.X(), min.Y(), min.Z()), dz, dy, material));
            sides.AddObject(new Plane(new Point(min.X(), max.Y(), max.Z()), dx, -dz, material));
            sides.AddObject(new Plane(new Point(min.X(), min.Y(), min.Z()), dx, dz, material));
        }

        public AABBox BoundingBox()
        {
            return sides.BoundingBox();
        }

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            return sides.Hit(ray, range, out hit);
        }
    }
}
