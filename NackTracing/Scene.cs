using ExportConfig;
using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.render.materials;
using NackEngine.core.render.materials.emissive;
using NackEngine.core.render.textures;
using NackEngine.core.space;
using NackEngine.IO;
using NackEngine.IO.loaders;
using NackEngine.math;
using NackEngine.objects;
using NackEngine.objects.modifiers;
using NackEngine.objects.volumes;
using System.Diagnostics;
using Color = NackEngine.core.render.Color;

namespace NackTracing
{
    using static NackEngine.core.space.NVector;
    using Point = NVector;

    public class Scene
    {
        public static bool previewMode = false;

        private static void Render(string title, Camera camera, Hittable world, Hittable lights = null)
        {
            if (previewMode)
            {
                Application.EnableVisualStyles();
                RenderWindow window = new RenderWindow(camera);
                Logger.Log("[INFO] Rendering...");
                Task.Run(() =>
                {
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        camera.RenderPreview(world, lights);
                        sw.Stop();
                        Logger.Log("[INFO] Render finished.");
                        ShowElapsedTime(sw);
                        Logger.Log("[INFO] Saving image...");
                        PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, title);
                        export.ExportFile(camera.PixelBuffer);
                        Logger.Log("[INFO] Image saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR EN RENDER]: {ex.Message}");
                    }
                });
                Application.Run(window);
            }
            else {
                Logger.Log("[INFO] Rendering...");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var render = camera.Render(world, lights);
                sw.Stop();
                Logger.Log("[INFO] Render finished.");
                ShowElapsedTime(sw);
                Logger.Log("[INFO] Saving image...");
                PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, title);
                export.ExportFile(render);
                Logger.Log("[INFO] Image saved successfully.");
            }
            Environment.Exit(0);
        }

        private static void ShowElapsedTime(Stopwatch sw)
        {
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{1:00}m:{2:00}s.{3:00}ms",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Logger.Log($"[INFO] Render completed in: {elapsedTime}");
        }

        public static void BasicScene()
        {
            HitCollection world = new HitCollection();

            var testTexture = new TestTexture(0.32f, Color.BLUE_NAVY, Color.WHITE);
            world.AddObject(new Sphere(new Point(0f, -1000f, 0f), 1000f, new Diffuse(testTexture)));

            float ballSize = 0.2f;
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    var chooseMat = MathSetting.RandomFloat();
                    var center = new Point(a + 0.9f * MathSetting.RandomFloat(), 0.2f, b + 0.9f * MathSetting.RandomFloat());

                    if ((center - new Point(4f, ballSize, 0f)).Length() > 0.9)
                    {
                        Material sphereMaterial;

                        if (chooseMat < 0.8f)
                        {
                            var albedo = NVector.Random() * NVector.Random();
                            sphereMaterial = new Diffuse(new Color(albedo));
                            var center2 = center + new NVector(0f, MathSetting.RandomFloat(0f, 0.5f), 0f);
                            world.AddObject(new Sphere(center, center2, 0.2f, sphereMaterial));
                        }
                        else if (chooseMat < 0.95f)
                        {
                            var albedo = NVector.Random(0.5f, 1f);
                            var fuzz = MathSetting.RandomFloat(0f, 0.5f);
                            sphereMaterial = new Metal(new Color(albedo), fuzz);
                        }
                        else
                        {
                            sphereMaterial = new Dielectric(1.5f);
                        }
                        world.AddObject(new Sphere(center, ballSize, sphereMaterial));
                    }
                }
            }
            Material material1 = new Dielectric(1.5f);
            world.AddObject(new Sphere(new Point(0f, 1f, 0f), 1.0f, material1));

            Material material2 = new Diffuse(new Color(0.4f, 0.2f, 0.1f));
            world.AddObject(new Sphere(new Point(-4f, 1f, 0f), 1.0f, material2));

            Material material3 = new Metal(new Color(0.7f, 0.6f, 0.5f), 0.0f);
            world.AddObject(new Sphere(new Point(4f, 1f, 0f), 1.0f, material3));

            Camera camera = new Camera(
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20,

                depthFieldAngle: 0.6f,
                focusDistance: 10.0f
            );
            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            var bvhWorld = new BVHNode(world);

            Render("BasicScene", camera, bvhWorld);
        }

        public static void CheckeredSpheres()
        {
            HitCollection world = new HitCollection();

            var testTexture = new TestTexture(0.32f, Color.BLUE_NAVY, Color.WHITE);

            world.AddObject(new Sphere(new Point(0, -10, 0), 10, new Diffuse(testTexture)));
            world.AddObject(new Sphere(new Point(0, 10, 0), 10, new Diffuse(testTexture)));

            Camera camera = new Camera(
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20,

                depthFieldAngle: 0
            );
            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );
            Render("CheckeredSpheres", camera, world);
        }

        public static void EarthAndMars()
        {
            HitCollection world = new HitCollection();

            var earthTexture = ImageLoader.Load("EARTH");
            var marsTexture = ImageLoader.Load("MARS");
            var earthSurface = new Diffuse(earthTexture);
            var marsSurface = new Diffuse(marsTexture);

            world.AddObject(new Sphere(new Point(0, 2, 0), 1.5f, earthSurface));
            world.AddObject(new Sphere(new Point(0, -2, 0), 1.5f, marsSurface));

            Camera camera = new Camera(
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20,

                depthFieldAngle: 0
            );
            camera.SetLookPoint(
                    new Point(0, 0, 12), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );
            Render("EarthAndMars", camera, world);
        }

        public static void PerlinTest()
        {
            HitCollection world = new HitCollection();

            var perlinTexture = new NoiseTexture(4);
            var perlinSurface = new Diffuse(perlinTexture);
            world.AddObject(new Sphere(new Point(0, -1000, 0), 1000, perlinSurface));
            world.AddObject(new Sphere(new Point(0, 2, 0), 2, perlinSurface));

            Camera camera = new Camera(
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20,

                depthFieldAngle: 0
            );
            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            var bvhWorld = new BVHNode(world);

            Render("PerlinTest", camera, bvhWorld);
        }

        public static void PlanesScene()
        {
            HitCollection world = new HitCollection();

            var dollarTexture = new Diffuse(ImageLoader.Load("DOLLAR"));

            world.AddObject(new Plane(new Point(-3, -2, 5),
                    new NVector(0, 0, -4), new NVector(0, 4 / 2, 0),
                    dollarTexture));
            world.AddObject(new Plane(new Point(-2, -2, 0),
                     new NVector(4, 0, 0), new NVector(0, 4 / 2, 0),
                     dollarTexture));
            world.AddObject(new Plane(new Point(3, -2, 1),
                     new NVector(0, 0, 4), new NVector(0, 4 / 2, 0),
                     dollarTexture));
            world.AddObject(new Plane(new Point(-2, 3, 1),
                     new NVector(4, 0, 0), new NVector(0, 0, 4 / 2),
                     dollarTexture));
            world.AddObject(new Plane(new Point(-2, -3, 5),
                    new NVector(4, 0, 0), new NVector(0, 0, -4 / 2),
                    dollarTexture));

            Camera camera = new Camera(
                aspectRatio: 1.0f,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 80,

                depthFieldAngle: 0
            );
            camera.SetBackgroundColor(new Color(0.7f, 0.8f, 1.0f));
            camera.SetLookPoint(
                    new Point(0, 0, 9),
                    new Point(0, 0, 0),
                    new NVector(0, 1, 0)
            );

            var bvhWorld = new BVHNode(world);

            Render("PlanesScene", camera, bvhWorld);
        }

        public static void LightTest()
        {
            HitCollection world = new HitCollection();

            var perlinTexture = new NoiseTexture(4);

            world.AddObject(new Sphere(
                new Point(0, -1000, 0), 1000,
                new Diffuse(perlinTexture)));

            world.AddObject(new Sphere(
                new Point(0, 2, 0), 2,
                new Diffuse(perlinTexture)));

            var diffuseLight = new DiffuseLight(new Color(4, 4, 4));
            world.AddObject(new Sphere(
                new Point(0, 7, 0), 2,
                diffuseLight));
            world.AddObject(new Plane(new Point(3, 1, -2),
                new NVector(2, 0, 0), new NVector(0, 2, 0), diffuseLight));

            Camera camera = new Camera(
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20,

                depthFieldAngle: 0
            );
            camera.SetBackgroundColor(Color.BLACK);
            camera.SetLookPoint(
                    new Point(26, 3, 6), // Look point
                    new Point(0, 2, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            var bvhWorld = new BVHNode(world);

            Render("LightTest", camera, bvhWorld);
        }

        public static void CornellBox()
        {
            HitCollection world = new HitCollection();

            var red = new Diffuse(Color.RED_MODERN);
            var grey = new Diffuse(Color.GREY_LIGHT);
            var lime = new Diffuse(Color.GREEN_LIME);
            var light = new DiffuseLight(new Color(15, 15, 15));

            world.AddObject(new Plane(new Point(555, 0, 0),
                new NVector(0, 555, 0), new NVector(0, 0, 555),
                lime));

            world.AddObject(new Plane(new Point(0, 0, 0),
                new NVector(0, 555, 0), new NVector(0, 0, 555),
                red));

            var planeLight = new Plane(new Point(343, 554, 332),
                new NVector(-130, 0, 0), new NVector(0, 0, -105),
                light);

            world.AddObject(planeLight);

            world.AddObject(new Plane(new Point(0, 0, 0),
                new NVector(555, 0, 0), new NVector(0, 0, 555),
                grey));

            world.AddObject(new Plane(new Point(555, 555, 555),
                new NVector(-555, 0, 0), new NVector(0, 0, -555),
                grey));

            world.AddObject(new Plane(new Point(0, 0, 555),
                new NVector(555, 0, 0), new NVector(0, 555, 0),
                grey));

            Hittable box1 = new Box(new Point(0, 0, 0), new Point(165, 330, 165), grey);
            box1 = new Rotate(box1, 15.0f, Axis.Y);
            box1 = new Translate(box1, new Point(265, 0, 295));
            world.AddObject(box1);

            Hittable box2 = new Box(new Point(0, 0, 0), new Point(165, 165, 165), grey);
            box2 = new Rotate(box2, -18.0f, Axis.Y);
            box2 = new Translate(box2, new Point(130, 0, 65));
            world.AddObject(box2);

            Camera camera = new Camera(
                aspectRatio: 1.0f,
                imageWidth: 600,
                numSamples: 200,
                maxDepth: 50,
                fieldView: 40,

                depthFieldAngle: 0
            );
            camera.SetBackgroundColor(Color.BLACK);
            camera.SetLookPoint(
                    new Point(278, 278, -800), // Look point
                    new Point(278, 278, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            var bvhWorld = new BVHNode(world);

            Render("CornellBox", camera, bvhWorld);
        }

        public static void CornellSmoke()
        {
            HitCollection world = new HitCollection();

            var red = new Diffuse(Color.RED_MODERN);
            var grey = new Diffuse(Color.GREY_LIGHT);
            var lime = new Diffuse(Color.GREEN_LIME);
            var light = new DiffuseLight(new Color(7, 7, 7));

            world.AddObject(new Plane(new Point(555, 0, 0),
                new NVector(0, 555, 0), new NVector(0, 0, 555),
                lime));

            world.AddObject(new Plane(new Point(0, 0, 0),
                new NVector(0, 555, 0), new NVector(0, 0, 555),
                red));

            world.AddObject(new Plane(new Point(113, 554, 127),
                new NVector(330, 0, 0), new NVector(0, 0, 305),
                light));

            world.AddObject(new Plane(new Point(0, 555, 0),
                new NVector(555, 0, 0), new NVector(0, 0, 555),
                grey));

            world.AddObject(new Plane(new Point(0, 0, 0),
                new NVector(555, 0, 0), new NVector(0, 0, 555),
                grey));

            world.AddObject(new Plane(new Point(0, 0, 555),
                new NVector(555, 0, 0), new NVector(0, 555, 0),
                grey));

            Hittable box1 = new Box(new Point(0, 0, 0), new Point(160, 330, 165), grey);
            box1 = new Rotate(box1, 15.0f, Axis.Y);
            box1 = new Translate(box1, new Point(265, 0, 295));

            Hittable box2 = new Box(new Point(0, 0, 0), new Point(165, 165, 165), grey);
            box2 = new Rotate(box2, -18.0f, Axis.Y);
            box2 = new Translate(box2, new Point(130, 0, 65));

            world.AddObject(new ConstantVolume(box1, 0.01f, Color.BLACK));
            world.AddObject(new ConstantVolume(box1, 0.01f, Color.WHITE));

            Camera camera = new Camera(
                aspectRatio: 1.0f,
                imageWidth: 600,
                numSamples: 200,
                maxDepth: 50,
                fieldView: 40,

                depthFieldAngle: 0
            );
            camera.SetLookPoint(
                    new Point(278, 278, -800), // Look point
                    new Point(278, 278, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            var bvhWorld = new BVHNode(world);

            Render("CornellSmoke", camera, bvhWorld);
        }

        public static void FinalScene(int imageWidth, int numSamples, int maxDepth)
        {
            HitCollection boxes = new HitCollection();
            var ground = new Diffuse(new Color("#C2E2FA"));

            int numBoxes = 20;
            for (int i = 0; i < numBoxes; i++)
            {
                for (int j = 0; j < numBoxes; j++)
                {
                    float w = 100.0f;
                    float x0 = -1000.0f + i * w;
                    float z0 = -1000.0f + j * w;
                    float y0 = 0.0f;
                    var x1 = x0 + w;
                    var y1 = MathSetting.RandomFloat(1, 101);
                    var z1 = z0 + w;

                    boxes.AddObject(new Box(new Point(x0, y0, z0), new Point(x1, y1, z1), ground));
                }
            }

            HitCollection world = new HitCollection();

            world.AddObject(new BVHNode(boxes));

            var light = new DiffuseLight(new Color(7, 7, 7));
            var planeLight = new Plane(new Point(123, 554, 147),
                    new NVector(300, 0, 0), new NVector(0, 0, 265), light
            );
            world.AddObject(planeLight);

            var center1 = new Point(400, 400, 200);
            var center2 = center1 + new NVector(30, 0, 0);
            var sphereMaterial = new Diffuse(new Color("#FF5B5B"));
            world.AddObject(new Sphere(center1, center2, 50, sphereMaterial));

            world.AddObject(new Sphere(new Point(260, 150, 45), 50, new Dielectric(1.5f)));
            world.AddObject(new Sphere(
                new Point(0, 150, 145), 50, new Metal(new Color(0.8f, 0.8f, 0.9f), 1.0f)
            ));

            var boundary = new Sphere(new Point(360, 150, 145), 70, new Dielectric(1.5f));
            world.AddObject(boundary);
            world.AddObject(new ConstantVolume(boundary, 0.2f, new Color(0.2f, 0.4f, 0.9f)));

            boundary = new Sphere(new Point(0, 0, 0), 5000, new Dielectric(1.5f));
            world.AddObject(new ConstantVolume(boundary, 0.0001f, Color.WHITE));

            var earthMat = new Diffuse(ImageLoader.Load("MARS"));
            world.AddObject(new Sphere(new Point(400, 200, 400), 100, earthMat));

            var perlinTexture = new NoiseTexture(0.1f);
            world.AddObject(new Sphere(new Point(220, 280, 300), 80, new Diffuse(perlinTexture)));

            HitCollection boxes2 = new HitCollection();
            var fuzz = MathSetting.RandomFloat(0, 0.5f);
            var gold = new Metal(Color.GOLD, fuzz);
            int ns = 1000;
            for (int j = 0; j < ns; j++)
            {
                boxes2.AddObject(new Sphere(NVector.Random(0, 165), 10, gold));
            }

            Hittable boxNode = new BVHNode(boxes2);
            Hittable rotatedBox = new Rotate(boxNode, 15, Axis.Y);
            Hittable translatedBox = new Translate(rotatedBox, new NVector(-100, 270, 395));

            world.AddObject(translatedBox);

            Camera camera = new Camera(
                aspectRatio: 1.0f,
                imageWidth: imageWidth,
                numSamples: numSamples,
                maxDepth: maxDepth,
                fieldView: 40,

                depthFieldAngle: 0
            );
            camera.SetBackgroundColor(Color.BLACK);
            camera.SetLookPoint(
                    new Point(478, 278, -600), // Look point
                    new Point(278, 278, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            var bvhWorld = new BVHNode(world);

            Render("FinalScene", camera, bvhWorld);
        }

        public static void CPU_NACK()
        {
            HitCollection world = new HitCollection();

            var groundMaterial = new Diffuse(Color.GREY_DARK);
            world.AddObject(new Plane(
                new Point(-500, -10, -500),
                new NVector(1000, 0, 0),
                new NVector(0, 0, 1000),
                groundMaterial
            ));

            Material blue = new Diffuse(Color.BLUE_NAVY);
            HitCollection CPUObj = OBJLoader.Load("CPU_NACK", blue);

            var bvhCPU = new BVHNode(CPUObj);

            world.AddObject(bvhCPU);

            HitCollection lights = new HitCollection();
            var lightMaterial = new DiffuseLight(new Color(3, 3, 3));

            var ceilingLight = new Plane(
                new Point(-5, 8, -5),
                new NVector(10, 0, 0),
                new NVector(0, 0, 10),
                lightMaterial
            );

            var smallLight = new Plane(
                new Point(-3, 6.75f, -3),
                new NVector(10, 0, 0),
                new NVector(0, 0, 10),
                lightMaterial
            );

            world.AddObject(ceilingLight);
            lights.AddObject(ceilingLight);
            world.AddObject(smallLight);
            lights.AddObject(smallLight);

            Camera camera = BlenderAdapter.CreateCamera(
                // Location Camera in Blender
                X: -9.2436f,
                Y: -12.738f,
                Z: 9.0725f,

                // Location of the object to view
                targetX: 0,
                targetY: 0,
                targetZ: 0,

                // Lens properties
                focalLengthMM: 50.0f,

                // Render properties
                aspectRatio: 16.0f / 9.0f,
                imageWidth: 1080,
                numSamples: 1200
            );
            camera.SetBackgroundColor(Color.BLACK);

            var bvhWorld = new BVHNode(world);

            Render("CPU_NACK", camera, bvhWorld, lights);
        }

        public static void SALVAVIDAS()
        {
            HitCollection world = new HitCollection();

            var groundMaterial = new Diffuse(Color.GREY_DARK);
            world.AddObject(new Plane(
                new Point(-500, -10, -500),
                new NVector(1000, 0, 0),
                new NVector(0, 0, 1000),
                groundMaterial
            ));

            Material blue = new Diffuse(Color.BLUE_NAVY);
            HitCollection SalvavidasObj = OBJLoader.Load("SALVAVIDAS", blue);

            var bvhSalvavidas = new BVHNode(SalvavidasObj);

            world.AddObject(bvhSalvavidas);

            var lightMaterial = new DiffuseLight(new Color(10, 10, 10));

            var ceilingLight = new Plane(
                new Point(-5, 8, -5),
                new NVector(10, 0, 0),
                new NVector(0, 0, 10),
                lightMaterial
            );

            world.AddObject(ceilingLight);
            HitCollection lights = new HitCollection();
            lights.AddObject(ceilingLight);

            float zoom = 1.5f;

            Camera camera = BlenderAdapter.CreateCamera(
                // Location Camera in Blender
                X: -5.52395f * zoom,
                Y: -8.95188f * zoom,
                Z: 1.51004f * zoom,

                // Location of the object to view
                targetX: 0,
                targetY: 0,
                targetZ: 0,

                // Lens properties
                focalLengthMM: 50.0f,

                // Render properties
                aspectRatio: 1.0f,
                imageWidth: 1080,
                numSamples: 500
            );
            camera.SetBackgroundColor(Color.BLACK);

            var bvhWorld = new BVHNode(world);

            Render("SALVAVIDAS", camera, bvhWorld, lights);
        }

        public static void GPU_SCENE()
        {
            HitCollection world = new HitCollection();

            var groundMaterial = new Diffuse(Color.GREY_DARK);
            world.AddObject(new Plane(
                new Point(-500, -10, -500),
                new NVector(1000, 0, 0),
                new NVector(0, 0, 1000),
                groundMaterial
            ));

            Material blue = new Diffuse(Color.BLUE_NAVY);
            HitCollection GPUObj = OBJLoader.Load("GPU_SCENE", blue);

            var bvhGPU = new BVHNode(GPUObj);

            world.AddObject(bvhGPU);

            float zoom = 0.9f;

            Camera camera = BlenderAdapter.CreateCamera(
                // Location Camera in Blender
                X: -0.811323f * zoom,
                Y: -9.68943f * zoom,
                Z: 0.477847f * zoom,

                // Location of the object to view
                targetX: 1.53376f,
                targetY: -6.12449f,
                targetZ: 0.767201f,

                // Lens properties
                focalLengthMM: 36f,

                // Render properties
                aspectRatio: 16f / 9f,
                imageWidth: 1920,
                numSamples: 1000
            );

            Texture HDRI = HDRLoader.Load("SimonsRocks");
            camera.SetEnvironment(HDRI, 135f);

            var bvhWorld = new BVHNode(world);

            Render("GPU_SCENE", camera, bvhWorld);
        }
    }
}
