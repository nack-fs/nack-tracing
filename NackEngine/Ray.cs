using System;
using System.Collections.Generic;
using System.Text;

using Point = NackEngine.NVector;

namespace NackEngine
{
    public struct Ray
    {
        private Point origin;
        private NVector direction;

        public Ray() { }

        public Ray(Point origin, Point direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Point Origin() {
            return origin;    
        }

        public NVector Direction(){
            return direction;
        }

        public Point At(double t) { 
            return origin + t * direction;
        }
    }
}
