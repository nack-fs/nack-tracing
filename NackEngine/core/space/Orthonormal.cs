namespace NackEngine.core.space
{
    public struct Orthonormal
    {
        public NVector U { get; }
        public NVector V { get; }
        public NVector W { get; }

        public Orthonormal(NVector n)
        {
            this.W = NVector.UnitVector(n);
            NVector a = (Math.Abs(W.X()) > 0.9f) ? new NVector(0f, 1f, 0f) : new NVector(1f, 0f, 0f);
            V = NVector.UnitVector(NVector.Cross(W, a));
            U = NVector.Cross(W, V);
        }

        public NVector Local(float a, float b, float c)
        {
            return a * U + b * V + c * W;
        }

        public NVector Local(NVector a)
        {
            return a.X() * U + a.Y() * V + a.Z() * W;
        }
    }
}
