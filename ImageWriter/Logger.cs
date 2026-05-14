namespace NackEngine.IO
{
    public class Logger
    {
        private static (string tag, ConsoleColor color)[] levels = {
                ("[INFO]",  ConsoleColor.Yellow),
                ("[ERROR]", ConsoleColor.Red),
                ("[WARN]",  ConsoleColor.Gray)
        };

        public static void Log(string msg)
        {
            foreach (var level in levels)
            {
                if (msg.StartsWith(level.tag))
                {
                    Console.ForegroundColor = level.color;
                    Console.Write(level.tag);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(msg.Substring(level.tag.Length));
                    return;
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
        }

    }
}
