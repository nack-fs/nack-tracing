using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

using Point = NackEngine.core.space.NVector;

namespace NackEngine.core.physics
{
    public struct Ray
    {
        private Point origin;
        private NVector direction;
        private double timeMilis;

        public Ray() { }

        public Ray(Point origin, Point direction) : this(origin, direction, 0) { }

        public Ray(Point origin, Point direction, double timeMilis)
        {
            this.origin = origin;
            this.direction = direction;
            this.timeMilis = timeMilis;
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

        public double Time() {
            return timeMilis;
        }
    }
}
