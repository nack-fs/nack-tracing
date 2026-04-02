using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.space;
using Axis = NackEngine.core.space.NVector.Axis;
using Range = NackEngine.core.space.Range;

namespace NackEngine.objects.modifiers
{
    public class Rotate : Hittable
    {
        private Hittable wrappedObj;
        private Axis axis;
        private AABBox aabbox;

        private float sinTheta;
        private float cosTheta;

        public Rotate(Hittable wrappedObj, float angleDegrees, Axis axis)
        {
            this.wrappedObj = wrappedObj;
            this.axis = axis;

            float rad = float.DegreesToRadians(angleDegrees);
            this.sinTheta = MathF.Sin(rad);
            this.cosTheta = MathF.Cos(rad);

            this.aabbox = CalculateRotatedBox(wrappedObj.BoundingBox());
        }

        public AABBox BoundingBox() => aabbox;

        public bool Hit(Ray ray, Range range, out HitStruct hit)
        {
            var origin = ray.Origin().Rotate(-sinTheta, cosTheta, axis);
            var direction = ray.Direction().Rotate(-sinTheta, cosTheta, axis);

            Ray rotatedRay = new Ray(origin, direction, ray.Time());

            if (!wrappedObj.Hit(rotatedRay, range, out hit))
            {
                return false;
            }

            hit.Point = hit.Point.Rotate(sinTheta, cosTheta, axis);
            hit.Normal = hit.Normal.Rotate(sinTheta, cosTheta, axis);

            return true;
        }

        private AABBox CalculateRotatedBox(AABBox box)
        {
            var min = new NVector(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            var max = new NVector(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        float x = i * box.X.Max() + (1 - i) * box.X.Min();
                        float y = j * box.Y.Max() + (1 - j) * box.Y.Min();
                        float z = k * box.Z.Max() + (1 - k) * box.Z.Min();

                        NVector tester = new NVector(x, y, z).Rotate(sinTheta, cosTheta, axis);

                        for (int c = 0; c < 3; c++)
                        {
                            Axis ax = (Axis)c;
                            float val = tester.GetComponent(ax);

                            if (c == 0)
                            {
                                min = new NVector(MathF.Min(min.X(), val), min.Y(), min.Z());
                                max = new NVector(MathF.Max(max.X(), val), max.Y(), max.Z());
                            }
                            if (c == 1)
                            {
                                min = new NVector(min.X(), MathF.Min(min.Y(), val), min.Z());
                                max = new NVector(max.X(), MathF.Max(max.Y(), val), max.Z());
                            }
                            if (c == 2)
                            {
                                min = new NVector(min.X(), min.Y(), MathF.Min(min.Z(), val));
                                max = new NVector(max.X(), max.Y(), MathF.Max(max.Z(), val));
                            }
                        }
                    }
                }
            }

            return new AABBox(min, max);
        }

    }
}
