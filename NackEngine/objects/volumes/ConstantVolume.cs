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
        private double negInvDensity;
        private Material phaseFunction;

        public ConstantVolume(Hittable boundary, double density, Texture texture)
        {
            this.boundary = boundary;
            this.negInvDensity = -1/density;
            this.phaseFunction = new Isotropic(texture);
        }

        public ConstantVolume(Hittable boundary, double density, Color albedo)
        {
            this.boundary = boundary;
            this.negInvDensity = -1 / density;
            this.phaseFunction = new Isotropic(albedo);
        }

        public AABBox BoundingBox() => boundary.BoundingBox();

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            hit = default;
            HitStruct hit1, hit2;

            if (!boundary.Hit(ray, Range.UNIVERSE, out hit1)) { return false; }
            var tol = 1e-4;
            if (!boundary.Hit(ray, new Range(hit1.T + tol, double.MaxValue), out hit2)) { return false; }

            if (hit1.T < range.Min()) { hit1.T = range.Min(); }
            if (hit2.T > range.Max()) { hit2.T = range.Max(); }

            if (hit1.T >= hit2.T) { return false; }

            if (hit1.T < 0) { hit1.T = 0; }

            var rayLenght = ray.Direction().Length();
            var distanceInside = (hit2.T - hit1.T) * rayLenght;
            var hitDistance = negInvDensity * Math.Log(MathSetting.RandomDouble());

            if (hitDistance > distanceInside) { return false; }

            hit.T = hit1.T + hitDistance / rayLenght;
            hit.Point = ray.At(hit.T);

            hit.Normal = new NVector(1, 0, 0);
            hit.FrontFace = true;
            hit.Material = phaseFunction;

            return true;
        }
    }
}
