using System.Text.Json;

namespace NackEngine.IO
{
    public static class TextureConfig
    {
        private static Dictionary<string, string> textures;

        private static readonly string configPath = Path.Combine(AppContext.BaseDirectory, "assets", "nack-config.json");

        public static void Initialize()
        {
            if (textures != null) { return; }

            if (!File.Exists(configPath))
            {
                Console.WriteLine($" The configuration file was not found in {configPath}");
                textures = new Dictionary<string, string>();
                return;
            }

            LoadConfiguration();
        }

        private static void LoadConfiguration()
        {
            try
            {
                string json = File.ReadAllText(configPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine($"No configuration in the configuration file...");
                    textures = new Dictionary<string, string>();
                    return;
                }
                textures = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                Console.WriteLine($"Configuration loaded successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] reading the nack-configuration.json {e.Message}");
                textures = new Dictionary<string, string>();
            }
        }

        public static string GetImagePath(string texture)
        {
            if (textures == null) { Initialize(); }

            if (textures.TryGetValue(texture, out string path))
            {
                if (Path.IsPathRooted(path) && File.Exists(path)) { return path; }

                string relativePath = Path.Combine(AppContext.BaseDirectory, "assets", path);
                if (File.Exists(relativePath)) { return relativePath; }

                Console.WriteLine($"[WARN] La ruta del JSON para '{texture}' no existe.");
                return null;
            }

            if (File.Exists(texture))
            {
                return texture;
            }

            string localPath = Path.Combine(AppContext.BaseDirectory, "assets", texture);
            if (File.Exists(localPath))
            {
                return localPath;
            }
            return null;
        }
    }
}
