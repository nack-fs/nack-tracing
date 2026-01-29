using System.Text;
using System.IO;
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

            // World
            HitCollection world = new HitCollection();

            world.addObject(new Sphere(new Point(0,0,-1),0.5));
            world.addObject(new Sphere(new Point(0, -100.5, -1), 100));


            // Camera
            double aspectRatio = 16.0 / 9.0;
            int imageWidth = 400;

            Camera camera = new Camera(aspectRatio, imageWidth);
            camera.Render(world);
        }
    }
}