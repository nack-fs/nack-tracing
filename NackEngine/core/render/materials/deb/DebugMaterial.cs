using NackEngine.core.physics;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NackEngine.core.render.materials.debug
{
    public class DebugMaterial : Material
    {
        public bool Bounce(Ray ray, HitStruct hit, out ScatterStruct scatter)
        {
            scatter = default;
            return false;
        }

        public Color Emitted(double u, double v, Point point, HitStruct hit)
        {
            var normalColor = new NVector(hit.Normal.X() + 1, hit.Normal.Y() + 1, hit.Normal.Z() + 1) * 0.5;
            return new Color(normalColor);
        }
    }
}
