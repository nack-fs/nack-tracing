using NackEngine.core.space;

using Point = NackEngine.core.space.NVector;

namespace NackEngine.core.physics
{
    public struct Ray
    {
        private Point origin;
        private NVector direction;
        private float timeMilis;

        public Ray() { }

        public Ray(Point origin, Point direction) : this(origin, direction, 0) { }

        public Ray(Point origin, Point direction, float timeMilis)
        {
            this.origin = origin;
            this.direction = direction;
            this.timeMilis = timeMilis;
        }

        public Point Origin()
        {
            return origin;
        }

        public NVector Direction()
        {
            return direction;
        }

        public Point At(float t)
        {
            return origin + t * direction;
        }

        public float Time()
        {
            return timeMilis;
        }
    }
}
