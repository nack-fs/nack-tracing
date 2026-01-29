using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core
{
    using Point = NVector;
    public struct HitStruct
    {
        public Point point { set; get; }
        public NVector normal { set; get; }
        public double t { set; get; }

        public bool frontFace { set;  get; }

        public void setFaceNormal(Ray ray, NVector owNormal) {
            this.frontFace = NVector.Dot(ray.Direction(), owNormal) <0;
            this.normal = frontFace? owNormal : -owNormal;
        }
    }
}
