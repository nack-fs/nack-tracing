using NackEngine.core.physics;
using NackEngine.core.render.textures;
using NackEngine.math;

namespace NackEngine.core.render.materials
{
    public class Isotropic : Material
    {
        private Texture texture;

        public Isotropic(Texture texture)
        {
            this.texture = texture;
        }

        public Isotropic(Color albedo)
        {
            this.texture = new SolidColor(albedo);
        }

        public bool Bounce(Ray ray, HitStruct hit, out Color attenuation, out Ray bounced)
        {
            bounced = new Ray(hit.Point, MathSetting.RandomUnitVector(), ray.Time());
            attenuation = texture.Value(hit.U, hit.V, hit.Point);
            return true;
        }
    }
}
