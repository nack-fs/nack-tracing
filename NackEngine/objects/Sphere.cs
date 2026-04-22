using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.space;
using Range = NackEngine.core.space.Range;


namespace NackEngine.objects
{
    using Point = NVector;
    public class Sphere : Hittable
    {
        private Point center;
        private float radius;
        private Material material;
        private AABBox aabbox;

        private NVector movement;
        private bool isMoving;

        public Sphere(Point center, float radius, Material material)
        {
            this.center = center;
            this.radius = MathF.Max(0f, radius);
            this.material = material;

            InitializeAABBoxStatic(center, radius);
        }

        private void InitializeAABBoxStatic(Point center, float radius) {
            NVector rvec = new NVector(radius, radius, radius);
            this.aabbox = new AABBox(center - rvec, center + rvec);
        }

        // Constructor for a dynamic sphere in movement
        public Sphere(Point center1, Point center2, float radius, Material material)
        {
            this.center = center1;
            this.radius = Math.Max(0f, radius);
            this.material = material;

            this.movement = center2 - center1;
            this.isMoving = true;

            InitializeAABBoxDynamic(center1, center2, radius);
        }

        private void InitializeAABBoxDynamic(Point center1, Point center2, float radius)
        {
            NVector rvec = new NVector(radius, radius, radius);
            AABBox box1 = new AABBox(center1 - rvec, center1 + rvec);
            AABBox box2 = new AABBox(center2 - rvec, center2 + rvec);
            this.aabbox = new AABBox(box1, box2);
        }

        public bool Hit(Ray ray, Range rayT,out HitStruct hit)
        {
            Point center = GetCenter(ray.Time());

            NVector oc = center - ray.Origin();
            float a = ray.Direction().LengthSquared();
            float h = NVector.Dot(ray.Direction(), oc);
            var c = oc.LengthSquared() - radius * radius;
            hit = default;

            var discriminant = h * h - a * c;
            if (discriminant < 0) {
                return false;
            }

            float sqrtDis = MathF.Sqrt(discriminant);

            float root = (h - sqrtDis) / a;
            if (!rayT.Surrounds(root)) {
                root = (h + sqrtDis) / a;
                if (!rayT.Surrounds(root)) {
                    return false;
                }
            }

            hit.T = root;
            hit.Point = ray.At(hit.T);
            NVector owNormal = (hit.Point - center) / radius;
            hit.setFaceNormal(ray, owNormal);

            (float,float) uv = GetUV(owNormal);
            hit.U = uv.Item1;
            hit.V = uv.Item2;

            hit.Material = material;

            return true;
        }

        private Point GetCenter(float time)
        {
            if (!isMoving) return center;
            return center + (movement * time);
        }

        public AABBox BoundingBox() {
            return aabbox;
        }

        private (float u, float v) GetUV(Point point) {
            var theta = MathF.Acos(-point.Y());
            var phi = MathF.Atan2(-point.Z(), point.X()) + MathF.PI;

            float u = phi / (2f * MathF.PI);
            float v = theta / MathF.PI;

            return (u, v);
        }
    }
}
