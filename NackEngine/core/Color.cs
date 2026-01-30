using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
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

        public override string ToString() {
            var r = this.vector.X();
            var g = this.vector.Y();
            var b = this.vector.Z();

            Range intensity = new Range(0.000,0.999);
            int ir = (int)(256 * intensity.Clamp(r));
            int ig = (int)(256 * intensity.Clamp(g));
            int ib = (int)(256 * intensity.Clamp(b));

            return $"{ir} {ig} {ib}";
        }
    }
}
