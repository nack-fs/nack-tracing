using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    public class Range
    {
        private double min;
        private double max;

        // Default
        public static readonly Range EMPTY = new Range(double.MaxValue, double.MinValue);
        public static readonly Range UNIVERSE = new Range(double.MinValue, double.MaxValue);
        public static readonly Range DEFAULT = new Range(0, double.MaxValue);

        public Range()
        {
            this.min = double.MinValue; this.max = double.MaxValue;
        }

        public Range(double min, double max) {
            this.min = min; this.max=max;
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

        public double Clamp(double x)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }
    }
}
