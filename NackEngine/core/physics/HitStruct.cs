using NackEngine.core.render;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.physics
{
    using Point = NVector;
    public struct HitStruct
    {
        public Point Point { get; set; }
        public NVector Normal { get; set; }
        public double T { get; set; }

        // Cordinates 2D (U, V)
        public double U { get; set; }

        public double V { get; set; }

        public bool FrontFace { get; set; }
        public Material Material { get; set; }

        public void setFaceNormal(Ray ray, NVector owNormal) {
            this.FrontFace = NVector.Dot(ray.Direction(), owNormal) <0;
            this.Normal = FrontFace? owNormal : -owNormal;
        }
    }
}
