using NackEngine.core.space;

namespace NackEngine.core.render
{
    using Point = NVector;

    public interface Texture
    {
        public Color Value(double u, double v, Point point);
    }
}
