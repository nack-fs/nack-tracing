using System;
using System.Collections.Generic;
using ExportConfig;
using System.Text;

namespace NackEngine.core
{
    using Point = NVector;

    public class Camera
    {
        public double aspectRatio;
        public int imageWidth;
        // -----------------------
        private int imageHeight;
        private Point cameraOrigin;
        private Point pixel00;
        private NVector deltaH;
        private NVector deltaW;

        public Camera() { 
            this.aspectRatio = 1.0;
            this.imageWidth = 100;
        }

        public Camera(double aspectRatio, int imageWidth)
        {
            this.aspectRatio = aspectRatio;
            this.imageWidth = imageWidth;
        }

        public void Render(Hittable world) {
            Initialize();

            StringBuilder imageData = new StringBuilder();

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    var center = pixel00 + (x * deltaH) + (y * deltaW);
                    var rayDirection = center - cameraOrigin;
                    Ray r = new Ray(cameraOrigin, rayDirection);
                    Color color = RayColor(r, world);
                    imageData.AppendLine(color.ToString());
                }
            }
            PNGExport export = new PNGExport(imageWidth, imageHeight, "rendernew");
            export.ExportFile(imageData);
        }

        private void Initialize() {
            this.imageHeight = Math.Max(1, (int)(imageWidth / aspectRatio));

            // Camera
            var focalLenght = 1.0;
            var viewportHeight = 2.0;
            var viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);

            this.cameraOrigin = new Point(0, 0, 0);

            /*
             * Vectors
             * - Across the horizontal
             * - Down the vertical
             */
            var viewportH = new NVector(viewportWidth, 0, 0);
            var viewportW = new NVector(0, -viewportHeight, 0);


            // Horizontal and Vertical, Delta Vectors
            this.deltaH = viewportH / imageWidth;
            this.deltaW = viewportW / imageHeight;

            var viewportUpperLeft = cameraOrigin -
                new NVector(0, 0, focalLenght) - viewportH / 2 - viewportW / 2;

            this.pixel00 = viewportUpperLeft + 0.5 * (deltaH + deltaW);
        }

        private Color RayColor(Ray ray, Hittable world) {
            HitStruct hit;
            if (world.Hit(ray, Range.DEFAULT, out hit))
            {
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
