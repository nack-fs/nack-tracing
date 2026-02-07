using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;
using Range = NackEngine.core.space.Range;

namespace NackEngine.core.render
{
    public struct Color
    {
        private NVector vector;

        public Color(double r, double g, double b) {
            this.vector = new NVector(r, g, b);
        }

        public Color(NVector vector) {
            this.vector = vector;
        }

        public NVector Vector() { 
            return this.vector;
        }

        private double Linear2Gamma(double linear) {
            return (linear > 0) ? Math.Sqrt(linear) : 0;
        }

        public override string ToString() {
            var r = this.vector.X();
            var g = this.vector.Y();
            var b = this.vector.Z();

            r = Linear2Gamma(r);
            g = Linear2Gamma(g);
            b = Linear2Gamma(b);

            Range intensity = new Range(0.000,0.999);
            int ir = (int)(256 * intensity.Clamp(r));
            int ig = (int)(256 * intensity.Clamp(g));
            int ib = (int)(256 * intensity.Clamp(b));

            return $"{ir} {ig} {ib}";
        }

        public static Color operator *(Color a, Color b)
        {
            return new Color(a.vector * b.vector);
        }

        public static Color operator *(Color a, double b)
        {
            return new Color(a.vector * b);
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.vector + b.vector);
        }


    }
}
