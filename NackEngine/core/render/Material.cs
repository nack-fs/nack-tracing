using NackEngine.core.physics;
using NackEngine.core.space;

namespace NackEngine.core.render
{
    using Point = NVector;

    public interface Material
    {
        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation,
            out Ray bounced);

        public Color Emitted(double u, double v, Point point)
        {
            return new Color(0, 0, 0);
        }
    }
}
