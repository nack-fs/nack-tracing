namespace NackEngine.core.space
{
    public class Range
    {
        private float min;
        private float max;

        // Default
        public static readonly Range EMPTY = new Range(float.MaxValue, float.MinValue);
        public static readonly Range UNIVERSE = new Range(float.MinValue, float.MaxValue);
        public static readonly Range DEFAULT = new Range(0.001f, float.MaxValue);

        public Range()
        {
            this.min = float.MinValue; this.max = float.MaxValue;
        }

        public Range(float min, float max)
        {
            this.min = min; this.max = max;
        }

        public Range(Range a, Range b)
        {
            this.min = MathF.Min(a.Min(), b.Min());
            this.max = MathF.Max(a.Max(), b.Max());
        }

        public float Size()
        {
            return max - min;
        }

        public bool Contains(float z)
        {
            return min <= z && z <= max;
        }

        public bool Surrounds(double z)
        {
            return min < z && z < max;
        }

        public float Min()
        {
            return min;
        }

        public float Max()
        {
            return max;
        }

        public void SetMin(float min)
        {
            this.min = min;
        }

        public void SetMax(float max)
        {
            this.max = max;
        }

        public double Clamp(float x)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        public Range Expand(float delta)
        {
            var padding = delta * 0.5f;
            return new Range(min - padding, max + padding);
        }

        public static Range operator +(Range range, float displacement) =>
            new Range(range.min + displacement, range.max + displacement);
    }
}
