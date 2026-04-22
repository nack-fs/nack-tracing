using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.render.materials;
using NackEngine.core.space;
using NackEngine.math;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects.volumes
{
    public class ConstantVolume : Hittable
    {
        private Hittable boundary;
        private float negInvDensity;
        private Material phaseFunction;

        public ConstantVolume(Hittable boundary, float density, Texture texture)
        {
            this.boundary = boundary;
            this.negInvDensity = -1f/density;
            this.phaseFunction = new Isotropic(texture);
        }

        public ConstantVolume(Hittable boundary, float density, Color albedo)
        {
            this.boundary = boundary;
            this.negInvDensity = -1f / density;
            this.phaseFunction = new Isotropic(albedo);
        }

        public AABBox BoundingBox() => boundary.BoundingBox();

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            hit = default;
            HitStruct hit1, hit2;

            if (!boundary.Hit(ray, Range.UNIVERSE, out hit1)) { return false; }
            float tol = 1e-4f;
            if (!boundary.Hit(ray, new Range(hit1.T + tol, float.MaxValue), out hit2)) { return false; }

            if (hit1.T < range.Min()) { hit1.T = range.Min(); }
            if (hit2.T > range.Max()) { hit2.T = range.Max(); }

            if (hit1.T >= hit2.T) { return false; }

            if (hit1.T < 0) { hit1.T = 0; }

            float rayLenght = ray.Direction().Length();
            float distanceInside = (hit2.T - hit1.T) * rayLenght;
            float hitDistance = negInvDensity * MathF.Log(MathSetting.RandomFloat());

            if (hitDistance > distanceInside) { return false; }

            hit.T = hit1.T + hitDistance / rayLenght;
            hit.Point = ray.At(hit.T);

            hit.Normal = new NVector(1f, 0f, 0f);
            hit.FrontFace = true;
            hit.Material = phaseFunction;

            return true;
        }
    }
}
