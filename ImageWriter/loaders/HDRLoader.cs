using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core.render.textures;
using StbImageSharp;
using System;
using System.IO;

namespace NackEngine.IO.loaders
{
    public static class HDRLoader
    {
        public static FloatImageTexture Load(string filename, float? exposure = null) {

            string path = AssetConfig.GetHdriPath(filename);
            if (string.IsNullOrEmpty(path)) { path = filename; }

            try
            {
                using (var stream = File.OpenRead(path)) {
                    ImageResultFloat image = ImageResultFloat.FromStream(stream, ColorComponents.RedGreenBlue);
                    return exposure.HasValue
                        ? new FloatImageTexture(image.Data, image.Width, image.Height, exposure.Value)
                        : new FloatImageTexture(image.Data, image.Width, image.Height);
                }
            }
            catch (Exception e) {
                Logger.Log($"[ERROR] The HDRI ‘{filename}’ cannot be loaded: {e.Message}");
                return new FloatImageTexture(null, 0, 0);
            }
      
        }
    }
}
