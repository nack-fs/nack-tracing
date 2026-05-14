using NackEngine.core.render.textures.noise;
using NackEngine.core.space;

namespace NackEngine.core.render.textures
{
    public class NoiseTexture : Texture
    {
        private Perlin noise = new Perlin();
        private float scale;

        public NoiseTexture(float scale = 1f)
        {
            this.scale = scale;
        }

        public Color Value(float u, float v, NVector point)
        {
            return new Color(.5f, .5f, .5f) * (1f + MathF.Sin(scale * point.Z() + 10f * noise.Turbulence(point, 7)));
        }
    }
}
