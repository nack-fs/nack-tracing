using System.Text.Json;
using System.Text.Json.Serialization;

namespace NackEngine.IO
{
    public class NackConfigModel
    {
        [JsonPropertyName("assetsBaseConfig")]
        public string AssetsBaseConfig { get; set; }

        [JsonPropertyName("textures")]
        public Dictionary<string, string> Textures { get; set; }

        [JsonPropertyName("hdris")]
        public Dictionary<string, string> Hdris { get; set; }

        [JsonPropertyName("models")]
        public Dictionary<string, string> Models { get; set; }
    }

    public static class AssetConfig
    {
        private static NackConfigModel configData;

        private static readonly string assetsConfigPath = Path.Combine(AppContext.BaseDirectory, "assets", "nack-config.json");

        public static void Initialize()
        {
            if (configData != null) { return; }

            if (!File.Exists(assetsConfigPath))
            {
                Logger.Log($"[WARN] Configuration file not found in {assetsConfigPath}");
                CreateEmptyConfig();
                return;
            }

            LoadConfiguration();
        }

        private static void CreateEmptyConfig()
        {
            configData = new NackConfigModel
            {
                AssetsBaseConfig = "",
                Textures = new Dictionary<string, string>(),
                Hdris = new Dictionary<string, string>(),
                Models = new Dictionary<string, string>()
            };
        }

        private static void LoadConfiguration()
        {
            try
            {
                string json = File.ReadAllText(assetsConfigPath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                configData = JsonSerializer.Deserialize<NackConfigModel>(json, options);

                if (configData.Textures == null) { configData.Textures = new Dictionary<string, string>(); }
                if (configData.Hdris == null) { configData.Hdris = new Dictionary<string, string>(); }
                if (configData.Models == null) { configData.Models = new Dictionary<string, string>(); }

                Logger.Log($"[INFO] Configuration loaded successfully from {configData.AssetsBaseConfig}");
            }
            catch (Exception e)
            {
                Logger.Log($"[ERROR] Reading nack-config.json: {e.Message}");
                CreateEmptyConfig();
            }
        }

        private static string GetPathFromDictionary(string name, Dictionary<string, string> dict)
        {
            if (configData == null) { return null; }

            if (dict.TryGetValue(name, out string relativePath))
            {
                relativePath = relativePath.TrimStart('/', '\\');
                string fullPath = Path.Combine(configData.AssetsBaseConfig, relativePath);

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }

                Logger.Log($"[WARN] File named ‘{name}’ not found in {fullPath}");
                return null;
            }

            if (File.Exists(name))
            {
                return name;
            }

            if (!string.IsNullOrEmpty(configData.AssetsBaseConfig))
            {
                string fallback = Path.Combine(configData.AssetsBaseConfig, name);
                if (File.Exists(fallback)) { return fallback; }
            }
            return null;
        }

        public static string GetTexturePath(string name) => GetPathFromDictionary(name, configData?.Textures);
        public static string GetHdriPath(string name) => GetPathFromDictionary(name, configData?.Hdris);

        public static string GetModelPath(string name) => GetPathFromDictionary(name, configData?.Models);
    }
}
