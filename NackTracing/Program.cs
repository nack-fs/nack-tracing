using ExportConfig;
using NackEngine.core.physics;
using NackEngine.core.physics.bounding;
using NackEngine.core.render;
using NackEngine.core.render.materials;
using NackEngine.core.render.materials.debug;
using NackEngine.core.render.materials.emissive;
using NackEngine.core.render.textures;
using NackEngine.core.space;
using NackEngine.IO;
using NackEngine.math;
using NackEngine.objects;
using NackEngine.objects.modifiers;
using NackEngine.objects.volumes;
using System.Diagnostics;
using System.IO;
using System.Text;
using Range = NackEngine.core.space.Range;


namespace NackTracing
{
    using static NackEngine.core.space.NVector;
    using Point = NVector;

    internal class Program
    {
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            //BasicScene();
            //CheckeredSpheres();
            //EarthAndMars();
            //PerlinTest();
            //PlanesScene();
            //LightTest();
            //CornellBox();
            //CornellSmoke();
            //FinalScene(800, 1000, 20);
            //FinalScene(400, 250, 4);
            //FinalScene(250, 50, 4);
            //Monkey();
            CPU_NACK();

        }

        private static void BasicScene() {
            //// World
            HitCollection world = new HitCollection();

            var testTexture = new TestTexture(0.32, Color.BLUE_NAVY, Color.WHITE);
            world.AddObject(new Sphere(new Point(0, -1000, 0), 1000, new Diffuse(testTexture)));

            double ballSize = 0.2;
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    var chooseMat = MathSetting.RandomDouble();
                    var center = new Point(a + 0.9 * MathSetting.RandomDouble(), 0.2, b + 0.9 * MathSetting.RandomDouble());

                    if ((center - new Point(4, ballSize, 0)).Length() > 0.9)
                    {
                        Material sphereMaterial;

                        if (chooseMat < 0.8)
                        {
                            var albedo = NVector.Random() * NVector.Random();
                            sphereMaterial = new Diffuse(new Color(albedo));
                            var center2 = center + new NVector(0, MathSetting.RandomDouble(0, 0.5), 0);
                            world.AddObject(new Sphere(center, center2, 0.2, sphereMaterial));
                        }
                        else if (chooseMat < 0.95)
                        {
                            var albedo = NVector.Random(0.5, 1);
                            var fuzz = MathSetting.RandomDouble(0, 0.5);
                            sphereMaterial = new Metal(new Color(albedo), fuzz);
                        }
                        else
                        {
                            sphereMaterial = new Dielectric(1.5);
                        }
                        world.AddObject(new Sphere(center, ballSize, sphereMaterial));
                    }
                }
            }

            Material material1 = new Dielectric(1.5);
            world.AddObject(new Sphere(new Point(0, 1, 0), 1.0, material1));

            Material material2 = new Diffuse(new Color(0.4, 0.2, 0.1));
            world.AddObject(new Sphere(new Point(-4, 1, 0), 1.0, material2));

            Material material3 = new Metal(new Color(0.7, 0.6, 0.5), 0.0);
            world.AddObject(new Sphere(new Point(4, 1, 0), 1.0, material3));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 16.0 / 9.0,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0.6,
                focusDistance: 10.0
            );

            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void ShowElapsedTime(Stopwatch sw) {
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{1:00}m:{2:00}s.{3:00}ms",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine($"\nRender finalizado en: {elapsedTime}");
        }

        private static void CheckeredSpheres() {
            HitCollection world = new HitCollection();

            var testTexture = new TestTexture(0.32, Color.BLUE_NAVY, Color.WHITE);

            world.AddObject(new Sphere(new Point(0, -10, 0), 10, new Diffuse(testTexture)));
            world.AddObject(new Sphere(new Point(0, 10, 0), 10, new Diffuse(testTexture)));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 16.0 / 9.0,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void EarthAndMars() {
            HitCollection world = new HitCollection();

            var earthTexture = ImageLoader.Load("EARTH");
            var marsTexture = ImageLoader.Load("MARS");
            var earthSurface = new Diffuse(earthTexture);
            var marsSurface = new Diffuse(marsTexture);

            world.AddObject(new Sphere(new Point(0, 2, 0), 1.5, earthSurface));
            world.AddObject(new Sphere(new Point(0, -2, 0), 1.5, marsSurface));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 16.0 / 9.0,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetLookPoint(
                    new Point(0, 0, 12), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void PerlinTest()
        {
            HitCollection world = new HitCollection();

            var perlinTexture = new NoiseTexture(4);
            var perlinSurface = new Diffuse(perlinTexture);


            world.AddObject(new Sphere(new Point(0, -1000, 0), 1000, perlinSurface));
            world.AddObject(new Sphere(new Point(0, 2, 0), 2, perlinSurface));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 16.0 / 9.0,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void PlanesScene()
        {
            HitCollection world = new HitCollection();

            var dollarTexture = new Diffuse(ImageLoader.Load("DOLLAR_TEX"));


            world.AddObject(new Plane(new Point(-3, -2, 5),
                    new NVector(0,0,-4),new NVector(0, 4/2, 0),
                    dollarTexture));
            world.AddObject(new Plane(new Point(-2, -2, 0),
                     new NVector(4, 0, 0), new NVector(0, 4/2, 0),
                     dollarTexture));
            world.AddObject(new Plane(new Point(3, -2, 1),
                     new NVector(0, 0, 4),new NVector(0, 4/2, 0),
                     dollarTexture));
            world.AddObject(new Plane(new Point(-2, 3, 1),
                     new NVector(4, 0, 0),new NVector(0, 0, 4/2),
                     dollarTexture));
            world.AddObject(new Plane(new Point(-2, -3, 5),
                    new NVector(4, 0, 0),new NVector(0, 0, -4/2),
                    dollarTexture));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 1.0,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 80, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetBackgroundColor(new Color(0.7,0.8,1.0));

            camera.SetLookPoint(
                    new Point(0, 0, 9), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void LightTest()
        {
            HitCollection world = new HitCollection();

            var perlinTexture = new NoiseTexture(4);

            world.AddObject(new Sphere(
                new Point(0,-1000,0), 1000,
                new Diffuse(perlinTexture)));

            world.AddObject(new Sphere(
                new Point(0, 2, 0), 2,
                new Diffuse(perlinTexture)));

            var diffuseLight = new DiffuseLight(new Color(4,4,4));
            world.AddObject(new Sphere(
                new Point(0, 7, 0), 2,
                diffuseLight));
            world.AddObject(new Plane(new Point(3,1,-2),
                new NVector(2,0,0), new NVector(0, 2, 0), diffuseLight));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 16.0 / 9.0,
                imageWidth: 1080,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetBackgroundColor(Color.BLACK);

            camera.SetLookPoint(
                    new Point(26, 3, 6), // Look point
                    new Point(0, 2, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void CornellBox()
        {
            HitCollection world = new HitCollection();

            var red = new Diffuse(Color.RED_MODERN);
            var grey = new Diffuse(Color.GREY_LIGHT);
            var lime = new Diffuse(Color.GREEN_LIME);
            var light = new DiffuseLight(new Color(15, 15, 15));

            // Cornell Box

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

            // Boxes
            Hittable box1 = new Box(new Point(0, 0, 0), new Point(165, 330, 165), grey);
            box1 = new Rotate(box1, 15.0, Axis.Y);
            box1 = new Translate(box1, new Point(265, 0, 295));
            world.AddObject(box1);

            Hittable box2 = new Box(new Point(0, 0, 0), new Point(165, 165, 165), grey);
            box2 = new Rotate(box2, -18.0, Axis.Y);
            box2 = new Translate(box2, new Point(130, 0, 65));
            world.AddObject(box2);

            // Camera
            Camera camera = new Camera(
                aspectRatio: 1.0,
                imageWidth: 600,
                numSamples: 200,
                maxDepth: 50,
                fieldView: 40, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );
            camera.SetBackgroundColor(Color.BLACK);

            camera.SetLookPoint(
                    new Point(278, 278, -800), // Look point
                    new Point(278, 278, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void CornellSmoke() {
            HitCollection world = new HitCollection();

            var red = new Diffuse(Color.RED_MODERN);
            var grey = new Diffuse(Color.GREY_LIGHT);
            var lime = new Diffuse(Color.GREEN_LIME);
            var light = new DiffuseLight(new Color(7, 7, 7));

            // Cornell Box

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


            // Boxes
            Hittable box1 = new Box(new Point(0, 0, 0), new Point(160, 330, 165), grey);
            box1 = new Rotate(box1, 15.0, Axis.Y);
            box1 = new Translate(box1, new Point(265, 0, 295));

            Hittable box2 = new Box(new Point(0, 0, 0), new Point(165, 165, 165), grey);
            box2 = new Rotate(box2, -18.0, Axis.Y);
            box2 = new Translate(box2, new Point(130, 0, 65));

            world.AddObject(new ConstantVolume(box1, 0.01, Color.BLACK));
            world.AddObject(new ConstantVolume(box1, 0.01, Color.WHITE));

            // Camera
            Camera camera = new Camera(
                aspectRatio: 1.0,
                imageWidth: 600,
                numSamples: 200,
                maxDepth: 50,
                fieldView: 40, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetLookPoint(
                    new Point(278, 278, -800), // Look point
                    new Point(278, 278, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void FinalScene(int imageWidth, int numSamples, int maxDepth)
        {
            HitCollection boxes = new HitCollection();
            var ground = new Diffuse(new Color("#C2E2FA"));

            int numBoxes = 20;
            for (int i = 0; i < numBoxes; i++) {
                for (int j = 0; j < numBoxes; j++) {
                    var w = 100.0;
                    var x0 = -1000.0 + i * w;
                    var z0 = -1000.0 + j * w;
                    var y0 = 0.0;
                    var x1 = x0 + w;
                    var y1 = MathSetting.RandomDouble(1, 101);
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

            world.AddObject(new Sphere(new Point(260, 150, 45), 50, new Dielectric(1.5)));
            world.AddObject(new Sphere(
                new Point(0, 150, 145), 50, new Metal(new Color(0.8, 0.8, 0.9), 1.0)
            ));

            var boundary = new Sphere(new Point(360, 150, 145), 70, new Dielectric(1.5));
            world.AddObject(boundary);
            world.AddObject(new ConstantVolume(boundary, 0.2, new Color(0.2, 0.4, 0.9)));

            boundary = new Sphere(new Point(0, 0, 0), 5000, new Dielectric(1.5));
            world.AddObject(new ConstantVolume(boundary, 0.0001, Color.WHITE));

            var earthMat = new Diffuse(ImageLoader.Load("MARS"));
            world.AddObject(new Sphere(new Point(400, 200, 400), 100, earthMat));

            var perlinTexture = new NoiseTexture(0.1);
            world.AddObject(new Sphere(new Point(220, 280, 300), 80, new Diffuse(perlinTexture)));

            HitCollection boxes2 = new HitCollection();
            var fuzz = MathSetting.RandomDouble(0, 0.5);
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

            // Camera
            Camera camera = new Camera(
                aspectRatio: 1.0,
                imageWidth: imageWidth,
                numSamples: numSamples,
                maxDepth: maxDepth,
                fieldView: 40, // Zoom

                // Depth of field
                depthFieldAngle: 0
            );

            camera.SetBackgroundColor(Color.BLACK);

            camera.SetLookPoint(
                    new Point(478, 278, -600), // Look point
                    new Point(278, 278, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            // BVH World
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void Monkey()
        {
            //// World
            HitCollection world = new HitCollection();

            var testTexture = new TestTexture(0.32, Color.BLUE_NAVY, Color.WHITE);
            //world.AddObject(new Sphere(new Point(0, -1000, 0), 1000, new Diffuse(testTexture)));

            Material material1 = new Dielectric(1.5);
            //world.AddObject(new Sphere(new Point(0, 1, 0), 1.0, material1));

            Material red = new Diffuse(Color.RED_MODERN);
            //world.AddObject(new Sphere(new Point(-4, 1, 0), 1.0, material2));

            Material gold = new Metal(Color.GOLD, 0.0);
            //world.AddObject(new Sphere(new Point(4, 1, 0), 1.0, material3));


            HitCollection monkeyMesh = OBJLoader.Load("C:\\Users\\ignac\\Downloads\\monkey.obj", gold);

            // BVH World
            var bvhMonkey = new BVHNode(monkeyMesh);

            world.AddObject(bvhMonkey);

            // Camera
            Camera camera = new Camera(
                aspectRatio: 1.0,
                imageWidth: 1080,
                numSamples: 300,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0.6,
                focusDistance: 10.0
            );

            camera.SetLookPoint(
                    new Point(13, 2, 3), // Look point
                    new Point(0, 0, 0), // Look target
                    new NVector(0, 1, 0) // vup
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------

            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }

        private static void CPU_NACK() {
            HitCollection world = new HitCollection();

            Material white = new Diffuse(Color.BLUE_NAVY);
            HitCollection CPUObj = OBJLoader.Load("C:\\Users\\ignac\\Downloads\\CPU_FUTURE.obj", white);

            var bvhCPU = new BVHNode(CPUObj);

            world.AddObject(bvhCPU);

            Camera camera = BlenderAdapter.CreateCamera(
                // Location Camera in Blender
                X: -9.2436,
                Y: -12.738,
                Z: 9.0725,

                // Location of the object to view
                targetX: 0,
                targetY: 0,
                targetZ: 0,

                // Lens properties
                focalLengthMM: 50.0,

                // Render properties
                aspectRatio: 16.0 / 9.0,
                imageWidth: 200,
                numSamples: 100
            );

            Console.WriteLine("Iniciando render...");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // ---------- RENDER ------------
            var bvhWorld = new BVHNode(world);

            var render = camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);

            Console.WriteLine("Guardando imagen...");
            PNGExport export = new PNGExport(camera.imageWidth, camera.imageHeight, "rendernew");
            export.ExportFile(render);
        }
    }
}