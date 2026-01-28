using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine
{
    public struct Color
    {
        double r;
        double g;
        double b;

        public Color(double r, double g, double b) {
            this.r = r; this.g = g; this.b = b;
        }

        public override string ToString() {
            var r = Math.Clamp(this.r, 0.0, 0.999);
            var g = Math.Clamp(this.g, 0.0, 0.999);
            var b = Math.Clamp(this.b, 0.0, 0.999);

            int ir = (int)(255.999 * r);
            int ig = (int)(255.999 * g);
            int ib = (int)(255.999 * b);

            return $"{ir} {ig} {ib}";
        }
    }
}
