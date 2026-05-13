namespace NackTracing
{
    public static class DevConsole
    {
        private static readonly string releaseVersion = "9.0.1-RELEASE";
        private static readonly string buildDate = "2026.05.13";

        public static void DeveloperUserInterface()
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.Title = "NACK ENGINE | CORE RENDERER | DEVELOPER ACCESS";

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Separator('=');
                Console.ForegroundColor = ConsoleColor.DarkGray;
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
                Console.Write("MODE: DEVELOPER RENDER ENGINE");
                Console.WriteLine($" ] [ RELEASE: {releaseVersion} ]");
                Console.Write(" [ ");
                Console.Write($"BUILD DATE: {buildDate} ]");
                Console.WriteLine($" ] [ Threads Detected: {Environment.ProcessorCount} ]");
                Separator('-');

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n    > CORE INITIALIZED");
                Console.WriteLine("    > RENDER PIPELINE READY\n");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Separator('-');
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("    Nack Tracing Developer Access | Author: Ignacio Fernández Suárez");
                Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Separator('-');
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("    For more details, visit the repository: https://github.com/nack-fs/nack-tracing");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Separator('=');

                Console.WriteLine("");
            }
            catch (IOException)
            {
                Console.WriteLine("Default primary output.\n");
            }
        }

        private static void Separator(char lineChar)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(lineChar, 85));
            Console.ResetColor();
        }
    }
}
