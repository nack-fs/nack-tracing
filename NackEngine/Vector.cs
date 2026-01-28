namespace NackEngine
{
    public class Vector
    {
        private double[] v = new double[3];
        public Vector() {
            this.v = [0, 0, 0];
        }

        public Vector(double v1, double v2, double v3) {
            this.v[0] = v1;
            this.v[1] = v2;
            this.v[2] = v3;
        }

        public double X() { return this.v[0];}
            
    }
}
