using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine
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
            var r = Math.Clamp(this.vector.X(), 0.0, 0.999);
            var g = Math.Clamp(this.vector.Y(), 0.0, 0.999);
            var b = Math.Clamp(this.vector.Z(), 0.0, 0.999);

            int ir = (int)(255.999 * r);
            int ig = (int)(255.999 * g);
            int ib = (int)(255.999 * b);

            return $"{ir} {ig} {ib}";
        }
    }
}
