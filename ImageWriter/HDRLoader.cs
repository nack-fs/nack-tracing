using System;
using System.Collections.Generic;
using System.Text;
using NackEngine.core.render.textures;
using StbImageSharp;
using System;
using System.IO;

namespace NackEngine.IO
{
    public static class HDRLoader
    {
        public static FloatImageTexture Load(string filename) {

            string path = AssetConfig.GetHdriPath(filename);
            if (string.IsNullOrEmpty(path)) { path = filename; }

            try
            {
                using (var stream = File.OpenRead(path)) {
                    ImageResultFloat image = ImageResultFloat.FromStream(stream, ColorComponents.RedGreenBlue);
                    return new FloatImageTexture(image.Data, image.Width, image.Height);
                }
            }
            catch (Exception e) {
                Console.WriteLine($"[ERROR] No se puede cargar el HDRI '{filename}': {e.Message}");
                return new FloatImageTexture(null, 0, 0);
            }
      
        }
    }
}
