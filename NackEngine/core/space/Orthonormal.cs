using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.core.space
{
    public struct Orthonormal
    {
        public NVector U { get; }
        public NVector V { get; }
        public NVector W { get; }

        public Orthonormal(NVector n) { 
            this.W = NVector.UnitVector(n);
            NVector a = (Math.Abs(W.X()) > 0.9) ? new NVector(0, 1, 0) : new NVector(1, 0, 0);
            V = NVector.UnitVector(NVector.Cross(W, a));
            U = NVector.Cross(W, V);
        }

        public NVector Local(double a, double b, double c)
        {
            return a * U + b * V + c * W;
        }

        public NVector Local(NVector a)
        {
            return a.X() * U + a.Y() * V + a.Z() * W;
        }
    }
}
