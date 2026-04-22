using NackEngine.core.physics;
using NackEngine.core.render.textures;
using NackEngine.math;
using NackEngine.math.probdensities;

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

        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter)
        {
            scatter = new ScatterStruct();
            scatter.Attenuation = texture.Value(hit.U, hit.V, hit.Point);
            scatter.ProbDensity = new SphereProbDensity();
            scatter.SkipProb = false;
            scatter.Bounced = default;

            return true;
        }

        public float ScatterProb(Ray ray, HitStruct hit, Ray scattered)
        {
            return 1.0f / (4.0f * MathF.PI);
        }
    }
}
