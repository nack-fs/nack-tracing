using NackEngine.core.render.textures.noise;
using NackEngine.core.space;

namespace NackEngine.core.render.textures
{
    public class NoiseTexture : Texture
    {
        private Perlin noise = new Perlin();
        private double scale;

        public NoiseTexture(double scale = 1) {
            this.scale = scale;
        }

        public Color Value(double u, double v, NVector point)
        {
            return new Color(.5, .5, .5) * (1 + Math.Sin(scale * point.Z() + 10 * noise.Turbulence(point, 7)));
        }
    }
}
