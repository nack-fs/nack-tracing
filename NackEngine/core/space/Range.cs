using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.space
{
    public class Range
    {
        private double min;
        private double max;

        // Default
        public static readonly Range EMPTY = new Range(double.MaxValue, double.MinValue);
        public static readonly Range UNIVERSE = new Range(double.MinValue, double.MaxValue);
        public static readonly Range DEFAULT = new Range(0.001, double.MaxValue);

        public Range()
        {
            this.min = double.MinValue; this.max = double.MaxValue;
        }

        public Range(double min, double max) {
            this.min = min; this.max=max;
        }

        public Range(Range a, Range b)
        {
            this.min = Math.Min(a.Min(), b.Min());
            this.max = Math.Max(a.Max(), b.Max());
        }

        public double Size() {
            return max - min;
        }

        public bool Contains(double z) { 
            return min <= z && z <= max;
        }

        public bool Surrounds(double z) {
            return min < z && z < max;
        }

        public double Min() {
            return min;
        }

        public double Max() {
            return max;
        }

        public void SetMin(double min) {
            this.min = min;
        }

        public void SetMax(double max)
        {
            this.max = max;
        }

        public double Clamp(double x)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        public Range Expand(double delta) {
            var padding = delta / 2;
            return new Range(min - padding, max + padding);
        }
    }
}
