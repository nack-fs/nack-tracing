using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    using Point = NVector;
    public struct HitStruct
    {
        public Point Point { set; get; }
        public NVector Normal { set; get; }
        public double T { set; get; }

        public bool FrontFace { set;  get; }

        public Material Material { set; get; }

        public void setFaceNormal(Ray ray, NVector owNormal) {
            this.FrontFace = NVector.Dot(ray.Direction(), owNormal) <0;
            this.Normal = FrontFace? owNormal : -owNormal;
        }
    }
}
