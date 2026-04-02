using NackEngine.core.physics;
using NackEngine.core.render.materials;
using NackEngine.core.space;

namespace NackEngine.core.render
{
    using Point = NVector;

    public interface Material
    {
        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter);

        public float ScatterProb(Ray ray, HitStruct hit, Ray scattered) {
            return 0f;
        }

        public Color Emitted(float u, float v, Point point)
        {
            return new Color(0f, 0f, 0f);
        }
    }
}
