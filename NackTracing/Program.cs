using System.Text;
using System.IO;
using ExportConfig;
using NackEngine.core;
using NackEngine.objects;
using Range = NackEngine.core.Range;


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

            // World
            HitCollection world = new HitCollection();

            world.addObject(new Sphere(new Point(0,0,-1),0.5));
            world.addObject(new Sphere(new Point(0, -100.5, -1), 100));


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
                    Color color = rayColor(r, world);
                    imageData.AppendLine(color.ToString());
                }
            }
            PNGExport export = new PNGExport(imageWidth, imageHeight, "rendernew");
            export.ExportFile(imageData);
        }
        private static Color rayColor(Ray ray, Hittable world)
        {
            HitStruct hit;
            if (world.Hit(ray, Range.DEFAULT, out hit)) {
                return new Color(0.5 * (hit.normal + new Color(1, 1, 1).Vector()));
            }

            NVector unitDirection = NVector.UnitVector(ray.Direction());
            var a = 0.5 * (unitDirection.Y() + 1.0);
            Color white = new Color(1.0, 1.0, 1.0);
            Color lightBlue = new Color(0.5, 0.7, 1.0);
            return new Color(white.Vector() * (1.0 - a) + lightBlue.Vector() * a);
        }
    }
}