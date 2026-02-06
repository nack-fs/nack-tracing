using System.Text;
using System.IO;
using NackEngine.core;
using NackEngine.objects;
using Range = NackEngine.core.Range;
using NackEngine.materials;


namespace NackTracing
{
    using Point = NVector;

    internal class Program
    {
        static void Main(string[] args)
        {
            // Materials
            var groundMaterial = new Diffuse(new Color(0.8, 0.8, 0.0));
            var centerMaterial = new Diffuse(new Color(0.1, 0.2, 0.5));
            var leftMaterial = new Dielectric(1.5);
            var bubbleMaterial = new Dielectric(1.0 / 1.5);
            var rightMaterial = new Metal(new Color(0.8, 0.6, 0.2), 1.0);

            // World
            HitCollection world = new HitCollection();

            world.addObject(new Sphere(new Point(0.0, -100.5, -1.0), 100.0, groundMaterial));
            world.addObject(new Sphere(new Point(0.0, 0.0, -1.2), 0.5, centerMaterial));
            world.addObject(new Sphere(new Point(-1.0, 0.0, -1.0), 0.5, leftMaterial));
            world.addObject(new Sphere(new Point(-1.0, 0.0, -1.0), 0.4, bubbleMaterial));
            world.addObject(new Sphere(new Point(1.0, 0.0, -1.0), 0.5, rightMaterial));


            // Camera
            Camera camera = new Camera(
                aspectRatio: 16.0 / 9.0, 
                imageWidth: 400,
                numSamples:100, 
                maxDepth:50
            );

            camera.Render(world);
        }
    }
}