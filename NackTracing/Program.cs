using System.Text;
using System.IO;
using ExportConfig;
using NackEngine.core;

namespace NackTracing
{
    using Point = NVector;

    internal class Program
    {
        static void Main(string[] args)
        {
            // Image settings
            var aspectRatio = 16.0 / 9.0;
            int imageWidth = 400;

            int imageHeight = Math.Max(1,(int)(imageWidth / aspectRatio));

            // Camera
            var focalLenght = 1.0;
            var viewportHeight = 2.0;
            var viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);
            var cameraOrigin = new Point(0, 0, 0);

            /*
             * Vectors
             * - Across the horizontal
             * - Down the vertical
             */
            var viewportH = new NVector(viewportWidth,0,0);
            var viewportW = new NVector(0, -viewportHeight, 0);

            // Horizontal and Vertical, Delta Vectors
            var deltaH = viewportH / imageWidth;
            var deltaW = viewportW / imageHeight;

            var viewportUpperLeft = cameraOrigin -
                new NVector(0, 0, focalLenght) - viewportH / 2 - viewportW / 2;

            var pixel00 = viewportUpperLeft + 0.5 * (deltaH + deltaW);

            // Render
            StringBuilder imageData = new StringBuilder();

            for (int y = 0; y < imageHeight; y++) {
                for (int x = 0; x < imageWidth; x++) {
                    var center = pixel00 + (x*deltaH) + (y*deltaW);
                    var rayDirection = center - cameraOrigin;
                    Ray r = new Ray(cameraOrigin, rayDirection);
                    Color color = rayColor(r);
                    imageData.AppendLine(color.ToString());
                }
            }
            PNGExport export = new PNGExport(imageWidth, imageHeight, "rendernew");
            export.ExportFile(imageData);
        }
        private static Color rayColor(Ray ray)
        {
            var t = hitSphere(new Point(0, 0, -1), 0.5, ray);
            if (t > 0.0) {
                NVector n = NVector.UnitVector(ray.At(t) - new NVector(0, 0, -1));
                return new Color(0.5 * new Color(n.X()+1,n.Y()+1,n.Z()+1).Vector());
            }

            NVector unitDirection = NVector.UnitVector(ray.Direction());
            var a = 0.5 * (unitDirection.Y() + 1.0);
            Color white = new Color(1.0, 1.0, 1.0);
            Color lightBlue = new Color(0.5, 0.7, 1.0);
            return new Color(white.Vector() * (1.0 - a) + lightBlue.Vector() * a);
        }

        private static double hitSphere(Point center, double radius, Ray ray) {
            NVector oc = center - ray.Origin();
            var a = ray.Direction().LengthSquared();
            var h = NVector.Dot(ray.Direction(), oc);
            var c = oc.LengthSquared() - radius * radius;
            var discriminant = h * h - a * c;

            if (discriminant < 0){return -1.0;}
            return (h - Math.Sqrt(discriminant)) / a;
        }
    }
}