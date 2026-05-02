using NackEngine.core.physics;
using NackEngine.core.render.materials;
using NackEngine.core.space;

namespace NackEngine.core.render
{
    using Point = NVector;

    public interface Material
    {
        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter);

        public double ScatterProb(Ray ray, HitStruct hit, Ray scattered) {
            return 0;
        }

        public Color Emitted(double u, double v, Point point)
        {
            return new Color(0, 0, 0);
        }
    }
}
