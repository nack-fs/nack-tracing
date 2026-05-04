using NackEngine.core.physics;
using NackEngine.core.render.textures;
using NackEngine.core.space;
using NackEngine.math.probdensities;

namespace NackEngine.core.render.materials
{
    public class Diffuse : Material
    {
        private Texture texture;

        public Diffuse(Texture texture)
        {
            this.texture = texture;
        }

        public Diffuse(Color albedo) : this(new SolidColor(albedo)) { }

        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter)
        {
            scatter = new ScatterStruct();
            scatter.Attenuation = this.texture.Value(hit.U, hit.V, hit.Point);
            scatter.ProbDensity = new CosProbDensity(hit.Normal);
            scatter.SkipProb = false;
            scatter.Bounced = default;

            return true;
        }

        public float ScatterProb(Ray ray, HitStruct hit, Ray scattered)
        {
            var cos = NVector.Dot(hit.Normal, NVector.UnitVector(scattered.Direction()));
            return (cos < 0f) ? 0f : cos / MathF.PI;
        }
    }
}
