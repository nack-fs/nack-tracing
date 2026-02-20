using ExportConfig;
using NackEngine.core.physics;
using NackEngine.core.space;
using NackEngine.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace NackEngine.core.render
{
    using Point = NVector;
    using Range = space.Range;

    public class Camera
    {
        public double aspectRatio;
        public int imageWidth;
        public int numSamples;
        public int maxDepth;
        private Color background;

        public double fieldView;
        public Point lookPoint = new Point(0,0,0);
        public Point lookTarget = new Point(0, 0, -1);
        public NVector vup = new NVector(0,1,0);

        public double depthFieldAngle;
        public double focusDistance;

        // -----------------------

        private int imageHeight;
        private Point cameraOrigin;
        private Point pixel00;
        private NVector deltaH;
        private NVector deltaW;
        private double samplesScale;
        private NVector u, v, w;
        private NVector defocusDiskU;
        private NVector defocusDiskV;

        public Camera(double aspectRatio = 1.0, int imageWidth = 100, 
            int numSamples = 10, int maxDepth = 10, double fieldView = 90,
            double depthFieldAngle = 0, double focusDistance = 10)
        {
            this.aspectRatio = aspectRatio;
            this.imageWidth = imageWidth;
            this.numSamples = numSamples;
            this.maxDepth = maxDepth;
            this.fieldView = fieldView;
            this.depthFieldAngle = depthFieldAngle;
            this.focusDistance = focusDistance;

            this.background = Color.WHITE;
        }

        public void Render(Hittable world) {
            Initialize();

            Color[] pixelColors = new Color[imageWidth * imageHeight];

            Parallel.For(0, imageHeight, y => {
                for (int x = 0; x < imageWidth; x++)
                {
                    Color pixelColor = new Color(0, 0, 0);
                    for (int sample = 0; sample < numSamples; sample++)
                    {
                        Ray ray = GetRay(x, y);
                        pixelColor += RayColor(ray, maxDepth, world);
                    }
                    pixelColors[y * imageWidth + x] = pixelColor * samplesScale;
                }
            });

            StringBuilder imageData = new StringBuilder();

            for (int i = 0; i < pixelColors.Length; i++) {
                imageData.AppendLine(pixelColors[i].ToString());
            }

            PNGExport export = new PNGExport(imageWidth, imageHeight, "rendernew");
            export.ExportFile(imageData);
        }

        private void Initialize() {
            this.imageHeight = Math.Max(1, (int)(imageWidth / aspectRatio));

            // Viewport
            var angle = double.DegreesToRadians(fieldView);
            var h = Math.Tan(angle / 2);
            var viewportHeight = 2 * h * focusDistance;
            var viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);

            this.samplesScale = 1.0 / numSamples;
            this.cameraOrigin = lookPoint;

            // Camera coordinate frame (u,v,w)
            this.w = NVector.UnitVector(lookPoint - lookTarget);
            this.u = NVector.UnitVector(NVector.Cross(vup, w));
            this.v = NVector.Cross(w,u);

            /*
             * Vectors
             * - Across the horizontal
             * - Down the vertical
             */
            var viewportU = viewportWidth * u;
            var viewportV = viewportHeight * -v;


            // Horizontal and Vertical, Delta Vectors
            this.deltaH = viewportU / imageWidth;
            this.deltaW = viewportV / imageHeight;

            var viewportUpperLeft = cameraOrigin - (focusDistance * w)
                 - viewportU / 2 - viewportV / 2;

            this.pixel00 = viewportUpperLeft + 0.5 * (deltaH + deltaW);

            var defocusRadius = focusDistance * Math.Tan(double.DegreesToRadians(depthFieldAngle / 2));
            this.defocusDiskU = u * defocusRadius;
            this.defocusDiskV = v * defocusRadius;
        }

        public void SetLookPoint(Point lookPoint, Point lookTarget, NVector vup) {
            this.lookPoint = lookPoint;
            this.lookTarget = lookTarget;
            this.vup = vup;
        }

        private Color RayColor(Ray ray, int depth ,Hittable world) {
            if (depth <= 0) { return Color.BLACK; }

            HitStruct hit;
            if (!world.Hit(ray, Range.DEFAULT, out hit)) {
                return background;
            }
            Ray scattered;
            Color attenuation;
            bool hasScatter = hit.Material.Bounce(ray, hit, out attenuation, out scattered);

            Color colorEmitted = hit.Material.Emitted(hit.U, hit.V, hit.Point);

            if (!hasScatter) {
                return colorEmitted;
            }

            Color colorScatter = attenuation * RayColor(scattered, depth - 1, world);
            return colorEmitted + colorScatter;
        }

        private Ray GetRay(int x, int y) {
            var offset = Sample();
            var pixelSample = pixel00
                + ((x + offset.X()) * deltaH)
                + ((y + offset.Y()) * deltaW);
            var rayOrigin = (depthFieldAngle<=0)? cameraOrigin : DepthFieldDisk();
            var rayDirection = pixelSample - rayOrigin;
            var rayTime = MathSetting.RandomDouble();

            return new Ray(rayOrigin, rayDirection, rayTime);
        }

        private NVector Sample() {
            return new NVector(MathSetting.RandomDouble() - 0.5,
                MathSetting.RandomDouble() - 0.5, 0);
        }

        private NVector DepthFieldDisk() {
            var point = MathSetting.RandomUnitDisk();
            return cameraOrigin + (point.X() * defocusDiskU) + (point.Y() * defocusDiskV);
        }

        public void SetBackgroundColor(Color color) { 
            this.background = color;
        }
    }
}
