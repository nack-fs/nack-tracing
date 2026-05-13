using NackEngine.IO;
using System.Diagnostics;


namespace NackTracing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AssetConfig.Initialize();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            Scene.previewMode = true;

            DevConsole.DeveloperUserInterface();

            /*
             * Active Rendering Scene
             */
            Scene.CPU_NACK();

            /*
              Available Scenes to Test
              -------------------------
                Scene.BasicScene();
                Scene.EarthAndMars();
                Scene.PerlinTest();
                Scene.PlanesScene();
                Scene.LightTest();
                Scene.CornellBox();
                Scene.CornellSmoke();
                Scene.FinalScene(800, 1000, 20);

                Scene.CPU_NACK();
                Scene.SALVAVIDAS();
                Scene.GPU_SCENE();
             */
        }
    }
}