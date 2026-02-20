using System.Text;
using System.IO;
using NackEngine.objects;
using Range = NackEngine.core.space.Range;
using NackEngine.core.render.materials;
using System.Diagnostics;
using NackEngine.math;
using NackEngine.core.render;
using NackEngine.core.space;
using NackEngine.core.physics.bounding;
using NackEngine.core.render.textures;
using NackEngine.core.render.materials.emissive;


namespace NackTracing
{
    using Point = NVector;

    internal class Program
    {
        static void Main(string[] args)
        {
            //BasicScene();
            //CheckeredSpheres();
            //EarthAndMars();
            //PerlinTest();
            //PlanesScene();
            //LightTest();
            CornellBox();
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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
        }

        private static void EarthAndMars() {
            HitCollection world = new HitCollection();

            var earthTexture = new ImageTexture("EARTH");
            var marsTexture = new ImageTexture("MARS");
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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
        }

        private static void PlanesScene()
        {
            HitCollection world = new HitCollection();

            var dollarTexture = new Diffuse(new ImageTexture("DOLLAR_TEX"));


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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
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

            world.AddObject(new Plane(new Point(343, 554, 332),
                new NVector(-130, 0, 0), new NVector(0, 0, -105),
                light));

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
            world.AddObject(new Box(new Point(130,0,65), new Point(295,165,230),grey));
            world.AddObject(new Box(new Point(265, 0, 295), new Point(430, 330, 460), grey));

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

            camera.Render(bvhWorld);

            sw.Stop();
            ShowElapsedTime(sw);
        }
    }
}