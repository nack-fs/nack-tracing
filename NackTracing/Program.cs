using NackEngine.IO;
using System.Diagnostics;


namespace NackTracing
{
    internal class Program
    {
        private static readonly string releaseVersion = "8.05-STABLE";
        static void Main(string[] args)
        {
            AssetConfig.Initialize();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            Scene.previewMode = false;

            DeveloperUserInterface();

            for (int i = 0; i < 3; i++) {
                Scene.CornellBox();
                Scene.SALVAVIDAS();
                Scene.CPU_NACK();
            }
        }

        private static void DeveloperUserInterface()
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.Title = "NACK ENGINE | CORE RENDERER | DEVELOPER ACCESS";

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n    {new string('-', 50)}");
                Console.Write(" [ ");

                if (Scene.previewMode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("RENDER PREVIEW: ACTIVE");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("RENDER PREVIEW: INACTIVE");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" ] [ ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("MODE: DEVELOPER RENDER ENGINE");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" ] [ RELEASE: {releaseVersion} ]");
                Console.WriteLine($"\n    {new string('-', 80)}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n    > CORE INITIALIZED");
                Console.WriteLine("    > RENDER PIPELINE READY");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n    {new string('-', 80)}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("    Ray Tracing Environment | TFG - Ignacio Fernández Suárez | University of Oviedo");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n    {new string('-', 80)}");

                Console.WriteLine("");
            }
            catch (IOException)
            {
                Console.WriteLine("Default primary output.\n");
            }
        }
    }
}