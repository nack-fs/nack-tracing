using NackEngine.core.physics;
using NackEngine.core.render.textures;
using NackEngine.core.space;
using NackEngine.math;
using NackEngine.math.probdensities;

namespace NackEngine.core.render.materials
{
    public class Diffuse : Material
    {
        private Texture texture;
        private float specular;
        private float roughness;

        public Diffuse(Texture texture, 
            float specular=0f, float roughness=0f)
        {
            this.texture = texture;
            this.specular = Math.Clamp(specular, 0f, 1f);
            this.roughness = Math.Clamp(roughness, 0f, 1f);
        }

        public Diffuse(Color albedo, float specular = 0f, float roughness = 0f) 
            : this(new SolidColor(albedo), specular, roughness) { }

        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter)
        {
            scatter = new ScatterStruct();
            scatter.Attenuation = this.texture.Value(hit.U, hit.V, hit.Point);

            bool isSpecular = MathSetting.RandomFloat() < specular;

            if (isSpecular)
            {
                scatter.ProbDensity = null;
                scatter.SkipProb = true;
                scatter.Attenuation = Color.WHITE;

                NVector reflected = RayPhysics.Reflect(NVector.UnitVector(ray.Direction()), hit.Normal);
                NVector fuzz = NVector.UnitVector(reflected + (roughness * MathSetting.RandomUnitVector()));

                if (NVector.Dot(fuzz, hit.Normal) <= 0)
                {
                    scatter.Bounced = new Ray(hit.Point, reflected, ray.Time());
                }
                else
                {
                    scatter.Bounced = new Ray(hit.Point, fuzz, ray.Time());
                }
            }
            else {
                scatter.ProbDensity = new CosProbDensity(hit.Normal);
                scatter.SkipProb = false;
                scatter.Bounced = default;
            }

            return true;
        }

        public float ScatterProb(Ray ray, HitStruct hit, Ray scattered)
        {
            var cos = NVector.Dot(hit.Normal, NVector.UnitVector(scattered.Direction()));
            return (cos < 0f) ? 0f : cos / MathF.PI;
        }
    }
}
