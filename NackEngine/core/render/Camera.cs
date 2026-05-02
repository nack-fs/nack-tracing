using NackEngine.core.physics;
using NackEngine.core.render.materials;
using NackEngine.core.space;
using NackEngine.math;
using NackEngine.math.probdensities;


namespace NackEngine.core.render
{
    using Point = NVector;
    using Range = space.Range;

    public class Camera
    {
        public double aspectRatio;
        public int imageWidth;
        public int imageHeight;

        public int numSamples;
        public int maxDepth;
        private Color background;
        private double maxIntensity;

        public double fieldView;
        public Point lookPoint = new Point(0, 0, 0);
        public Point lookTarget = new Point(0, 0, -1);
        public NVector vup = new NVector(0, 1, 0);

        public double depthFieldAngle;
        public double focusDistance;

        // -----------------------

        private Point cameraOrigin;
        private Point pixel00;
        private NVector deltaH;
        private NVector deltaW;
        private double samplesScale;
        private NVector u, v, w;
        private NVector defocusDiskU;
        private NVector defocusDiskV;
        private int sqrtSPP;
        private double invSqrtSPP;

        private bool defaultBackground = true;
        private Texture environment = null;
        private double envSin = 0;
        private double envCos = 1;

        // -----------------------

        public Color[] PixelBuffer { get; private set; }
        public bool IsCompleted { get; set; } = false;
        public int CurrentSample { get; private set; }

        // -----------------------

        public Camera(double aspectRatio = 1, int imageWidth = 100,
            int numSamples = 10, int maxDepth = 10, double fieldView = 90,
            double depthFieldAngle = 0, double focusDistance = 10, double maxIntensity = 5)
        {
            this.aspectRatio = aspectRatio;
            this.imageWidth = imageWidth;
            this.numSamples = numSamples;
            this.maxDepth = maxDepth;
            this.fieldView = fieldView;
            this.depthFieldAngle = depthFieldAngle;
            this.focusDistance = focusDistance;
            this.maxIntensity = maxIntensity;

            this.background = Color.WHITE;
            this.imageHeight = Math.Max(1, (int)(imageWidth / aspectRatio));
        }

        public Color[] Render(Hittable world, Hittable lights = null)
        {
            Initialize();
            Color[] pixelColors = new Color[imageWidth * imageHeight];

            int rowsDone = 0;
            ShowProgress(() => rowsDone, imageHeight);

            Parallel.For(0, imageHeight, y =>
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    Color pixelColor = new Color(0, 0, 0);
                    for (int gridY = 0; gridY < sqrtSPP; gridY++)
                    {
                        for (int gridX = 0; gridX < sqrtSPP; gridX++)
                        {
                            Ray ray = GetRay(x, y, gridX, gridY);
                            Color sampleColor = RayColor(ray, maxDepth, world, lights);
                            if (!sampleColor.IsNaN())
                            {
                                pixelColor += sampleColor;
                            }
                        }
                    }
                    pixelColors[y * imageWidth + x] = pixelColor * samplesScale;
                }

                Interlocked.Increment(ref rowsDone);
            });
            return pixelColors;
        }

        public void RenderPreview(Hittable world, Hittable lights = null) {
            Initialize();

            int totalPixels = imageWidth * imageHeight;
            PixelBuffer = new Color[totalPixels];
            Color[] accumulationBuffer = new Color[totalPixels];

            int[] indexes = new int[totalPixels];
            for (int i = 0; i < totalPixels; i++) {
                indexes[i] = i;
            }

            for (int sample = 1; sample <= numSamples; sample++) {
                CurrentSample = sample;

                ShuffleIndexes(indexes);

                Parallel.ForEach(indexes, index =>
                {
                    int x = index % imageWidth;
                    int y = index / imageWidth;

                    Ray ray = GetRay(x, y, 0, 0);

                    Color sampleColor = RayColor(ray, maxDepth, world, lights);

                    if (!sampleColor.IsNaN())
                    {
                        accumulationBuffer[index] += sampleColor;
                    }

                    PixelBuffer[index] = accumulationBuffer[index] * (1.0 / sample);
                });

                IsCompleted = true;
                //Thread.Sleep(1);
            }
        }

        private void ShowProgress(Func<int> getRowsDone, int totalRows)
        {
            Task.Run(async () =>
            {
                int current = 0;
                while (current < totalRows)
                {
                    current = getRowsDone();
                    UpdateProgress(current, totalRows);
                }
                UpdateProgress(totalRows, totalRows);
            });
        }

        private void UpdateProgress(int current, int total)
        {
            double percent = (double)current / total * 100.0;
            Console.Title = $"Renderizando: {percent:F1}% ({current}/{total} filas)";
        }

        private void Initialize()
        {
            this.sqrtSPP = (int)Math.Sqrt(numSamples);
            this.samplesScale = 1 / (sqrtSPP * sqrtSPP);
            this.invSqrtSPP = 1 / sqrtSPP;

            // Viewport
            var angle = double.DegreesToRadians(fieldView);
            var h = Math.Tan(angle / 2);
            var viewportHeight = 2 * h * focusDistance;
            var viewportWidth = viewportHeight * ((double)imageWidth / imageHeight);

            this.cameraOrigin = lookPoint;

            // Camera coordinate frame (u,v,w)
            this.w = NVector.UnitVector(lookPoint - lookTarget);
            this.u = NVector.UnitVector(NVector.Cross(vup, w));
            this.v = NVector.Cross(w, u);

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

        public void SetLookPoint(Point lookPoint, Point lookTarget, NVector vup)
        {
            this.lookPoint = lookPoint;
            this.lookTarget = lookTarget;
            this.vup = vup;
        }

        private Color RayColor(Ray ray, int depth, Hittable world, Hittable lights = null)
        {
            if (depth <= 0) { return Color.BLACK; }

            HitStruct hit;
            if (!world.Hit(ray, Range.DEFAULT, out hit))
            {
                if (environment != null) {
                    NVector unitDirection = NVector.UnitVector(ray.Direction());
                    unitDirection = unitDirection.Rotate(envSin, envCos, NVector.Axis.Y);

                    double theta = Math.Acos(-unitDirection.Y());
                    double phi = Math.Atan2(-unitDirection.Z(), unitDirection.X()) + Math.PI;

                    double u = phi / (2 * Math.PI);
                    double v = theta / Math.PI;

                    return environment.Value(u, v, unitDirection);
                }

                if (defaultBackground)
                {
                    NVector unitDirection = NVector.UnitVector(ray.Direction());
                    double t = 0.5 * (unitDirection.Y() + 1.0);
                    return Color.WHITE * (1.0 - t) + new Color(0.5, 0.7, 1.0) * t;
                }
                return this.background;
            }
            Color colorEmitted = hit.Material.Emitted(hit.U, hit.V, hit.Point);

            ScatterStruct scatter;
            if (!hit.Material.Bounce(ray, hit, out scatter))
            {
                return colorEmitted;
            }

            if (scatter.SkipProb)
            {
                double offset = 1e-4;
                Ray offsetRay = new Ray(hit.Point + hit.Normal * offset, scatter.Bounced.Direction(), ray.Time());
                Color colorScatter = scatter.Attenuation * RayColor(offsetRay, depth - 1, world, lights);
                return colorEmitted + colorScatter;
            }
            else
            {
                ProbDensity p;
                if (lights == null)
                {
                    p = scatter.ProbDensity;
                }
                else
                {
                    var lightProb = new HittableProbDensity(lights, hit.Point);
                    p = new MixProbDensity(lightProb, scatter.ProbDensity);
                }

                double offset = 1e-4;
                Ray scatteredRay = new Ray(hit.Point + hit.Normal * offset, p.Generate(), ray.Time());

                double probability = p.Value(scatteredRay.Direction());

                double tol = 1e-8;
                if (probability <= tol) { return colorEmitted; }

                double scatterpdf = hit.Material.ScatterProb(ray, hit, scatteredRay);

                Color sampleColor = RayColor(scatteredRay, depth - 1, world, lights);
                if (sampleColor.IsNaN()) return colorEmitted;

                Color colorScatter = (scatter.Attenuation * scatterpdf * sampleColor) * (1.0 / probability);

                colorScatter = colorScatter.Clamp(0, maxIntensity);
                return colorEmitted + colorScatter;
            }
        }

        private Ray GetRay(int x, int y, int gridX, int gridY)
        {
            var offset = Sample(gridX, gridY);
            var pixelSample = pixel00
                + ((x + offset.X()) * deltaH)
                + ((y + offset.Y()) * deltaW);
            var rayOrigin = (depthFieldAngle <= 0) ? cameraOrigin : DepthFieldDisk();
            var rayDirection = pixelSample - rayOrigin;
            var rayTime = MathSetting.RandomDouble();

            return new Ray(rayOrigin, rayDirection, rayTime);
        }

        private NVector Sample(int gridX, int gridY)
        {
            double posXgrid = ((gridX + MathSetting.RandomDouble()) * invSqrtSPP) - 0.5;
            double posYgrid = ((gridY + MathSetting.RandomDouble()) * invSqrtSPP) - 0.5;
            return new NVector(posXgrid, posYgrid, 0);
        }

        private NVector DepthFieldDisk()
        {
            var point = MathSetting.RandomUnitDisk();
            return cameraOrigin + (point.X() * defocusDiskU) + (point.Y() * defocusDiskV);
        }

        public void SetBackgroundColor(Color color)
        {
            this.background = color;
            this.defaultBackground = false;
        }

        public void SetEnvironment(Texture hdri, double rotation)
        {
            this.environment = hdri;
            this.defaultBackground = false;

            double rad = double.DegreesToRadians(rotation);
            this.envSin = Math.Sin(rad);
            this.envCos = Math.Cos(rad);
        }

        private void ShuffleIndexes(int[] array) { 
            int n = array.Length;

            while (n > 1) {
                int k = Random.Shared.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
    }
}
