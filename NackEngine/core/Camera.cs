using ExportConfig;
using NackEngine.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NackEngine.core
{
    using Point = NVector;

    public class Camera
    {
        public double aspectRatio;
        public int imageWidth;
        public int numSamples;
        // -----------------------
        private int imageHeight;
        private Point cameraOrigin;
        private Point pixel00;
        private NVector deltaH;
        private NVector deltaW;
        private double samplesScale;

        public Camera() { 
            this.aspectRatio = 1.0;
            this.imageWidth = 100;
            this.numSamples = 10;
        }

        public Camera(double aspectRatio, int imageWidth, int numSamples)
        {
            this.aspectRatio = aspectRatio;
            this.imageWidth = imageWidth;
            this.numSamples = numSamples;
        }

        public void Render(Hittable world) {
            Initialize();

            StringBuilder imageData = new StringBuilder();

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    //var center = pixel00 + (x * deltaH) + (y * deltaW);
                    //var rayDirection = center - cameraOrigin;
                    //Ray r = new Ray(cameraOrigin, rayDirection);
                    //Color color = RayColor(r, world);
                    //imageData.AppendLine(color.ToString());
                    Color pixelColor = new Color(0, 0, 0);
                    for (int sample = 0; sample < numSamples; sample++) {
                        Ray ray = getRay(x, y);
                        pixelColor = new Color(pixelColor.Vector() + RayColor(ray, world).Vector());
                    }
                    imageData.AppendLine(new Color(samplesScale*pixelColor.Vector()).ToString());
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

            this.samplesScale = 1.0 / numSamples;
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

        private Ray getRay(int x, int y) {
            var offset = Sample();
            var pixelSample = pixel00
                + ((x + offset.X()) * deltaH)
                + ((y + offset.X()) * deltaW);
            var rayOrigin = cameraOrigin;
            var rayDirection = pixelSample - rayOrigin;
            return new Ray(rayOrigin, rayDirection);
        }

        private NVector Sample() {
            // From [-0.5,-0.5] to [0.5,0.5]
            return new NVector(MathSetting.RandomDouble() - 0.5,
                MathSetting.RandomDouble() - 0.5, 0);
        }
    }
}
