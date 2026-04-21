using NackEngine.core.space;
using NackEngine.IO;
using System.Diagnostics;


namespace NackTracing
{
    internal class Program
    {
        private static readonly string releaseVersion = "7.42-STABLE";
        static void Main(string[] args)
        {
            DeveloperUserInterface();

            AssetConfig.Initialize();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            //Scene.BasicScene();
            //Scene.CheckeredSpheres();
            //Scene.EarthAndMars();
            //Scene.PerlinTest();
            //Scene.PlanesScene();
            //Scene.LightTest();
            //Scene.CornellBox();
            //Scene.CornellSmoke();
            //Scene.FinalScene(800, 1000, 20);
            //Scene.FinalScene(400, 250, 4);
            //Scene.FinalScene(250, 50, 4);
            //Scene.Monkey();
            //Scene.CPU_NACK();
            //Scene.SALVAVIDAS();
            //Scene.GPU_SCENE();
            //Scene.SALVAVIDAS_HDRI();
            //Scene.CPU_NACK_LITE();

            Scene.PREVIEW_CPU();
            //Scene.CPU_NACK_LITE();
        }

        private static void DeveloperUserInterface()
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.Title = "NACK ENGINE | CORE RENDERER | DEVELOPER ACCESS";

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(" _____________________________________________________________________________");
                Console.Write(" [ ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("RENDERING: ACTIVE");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" ] [ ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("MODE: DEVELOPER RENDER ENGINE");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" ] [ RELEASE: {releaseVersion} ]");
                Console.WriteLine(" _____________________________________________________________________________\n");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n    > CORE_SUBSYSTEM_INITIALIZED...");
                Console.WriteLine("    > RENDER_PIPELINE_READY");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n    -------------------------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("    Ray Tracing Laboratory | TFG Ignacio Fernández | Univ. de Oviedo");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("    -------------------------------------------------------------------------");

                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[SYSTEM] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Console stream redirected to primary output.\n");
            }
            catch (IOException)
            {
                Console.WriteLine("Default primary output.\n");
            }
        }
    }
}