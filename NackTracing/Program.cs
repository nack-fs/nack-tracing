using System.Text;
using System.IO;
using NackEngine.objects;
using Range = NackEngine.core.space.Range;
using NackEngine.materials;
using System.Diagnostics;
using NackEngine.math;
using NackEngine.core.render;
using NackEngine.core.space;
using NackEngine.core.physics.bounding;


namespace NackTracing
{
    using Point = NVector;

    internal class Program
    {
        static void Main(string[] args)
        {
            //// World
            HitCollection world = new HitCollection();

            var groundMaterial = new Diffuse(new Color(0.5, 0.5, 0.5));
            world.AddObject(new Sphere(new Point(0, -1000, 0), 1000, groundMaterial));

            double ballSize = 0.2;
            for (int a = -11; a < 11; a++) {
                for (int b = -11; b < 11; b++) {
                    var chooseMat = MathSetting.RandomDouble();
                    var center = new Point(a+0.9* MathSetting.RandomDouble(), 0.2, b+ 0.9 * MathSetting.RandomDouble());

                    if ((center - new Point(4, ballSize, 0)).Length() > 0.9) {
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
                        else {
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
                imageWidth: 400,
                numSamples: 100,
                maxDepth: 50,
                fieldView: 20, // Zoom

                // Depth of field
                depthFieldAngle: 0.6,
                focusDistance: 10.0
            );
                
            camera.SetLookPoint(
                    new Point(13,2,3), // Look point
                    new Point(0,0,0), // Look target
                    new NVector(0,1,0) // vup
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
    }
}