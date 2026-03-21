using Assimp;
using NackEngine.core.render;
using NackEngine.core.render.materials;
using NackEngine.core.space;
using NackEngine.objects;

namespace NackEngine.IO
{
    using Material = core.render.Material;
    using Point = NVector;

    public static class OBJLoader
    {
        public static HitCollection Load(string filepath, Material defaultMaterial = null)
        {
            HitCollection world = new HitCollection();
            AssimpContext importer = new AssimpContext();

            try
            {
                Console.WriteLine($"Cargando modelo OBJ: {filepath}");

                Scene scene = importer.ImportFile(filepath,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.JoinIdenticalVertices
                );

                if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete)
                    || scene.RootNode == null)
                {
                    Console.WriteLine($"[ERROR] Fallo al procesar el OBJ: {filepath}");
                    return world;
                }

                string basePath = Path.GetDirectoryName(filepath);
                List<Material> materials = new List<Material>();

                if (scene.HasMaterials) {
                    foreach (var mat in scene.Materials) {
                        materials.Add(ConvertMaterial(mat, basePath, defaultMaterial));
                    }
                }

                int totalTriangles = 0;
                foreach (var mesh in scene.Meshes)
                {
                    Material actualMaterial = defaultMaterial;
                    if (mesh.MaterialIndex >= 0 && mesh.MaterialIndex < materials.Count) {
                        actualMaterial = materials[mesh.MaterialIndex] ?? defaultMaterial;
                    }

                    Point[] vertices = new Point[mesh.VertexCount];

                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        vertices[i] = new Point(mesh.Vertices[i].X,
                                                mesh.Vertices[i].Y,
                                                mesh.Vertices[i].Z);
                    }

                    foreach (var face in mesh.Faces)
                    {
                        if (face.IndexCount == 3)
                        {
                            Point v0 = vertices[face.Indices[0]];
                            Point v1 = vertices[face.Indices[1]];
                            Point v2 = vertices[face.Indices[2]];

                            world.AddObject(new Triangle(v0, v1, v2, actualMaterial));

                            totalTriangles++;
                        }
                    }
                }
                Console.WriteLine($"Modelo OBJ cargado correctamente. Total triángulos: {totalTriangles}");
                Console.WriteLine($"Material MTL cargado correctamente. Total materiales: {materials.Count}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Cargando el OBJ: {e.Message}");
            }
            return world;
        }

        private static Material ConvertMaterial(Assimp.Material mtl, string basePath, Material def) {
            string matName = mtl.Name.ToLower();
            bool isExplicitMetal = matName.Contains("metal");
            bool isExplicitGlass = matName.Contains("glass");


            if (isExplicitGlass || (mtl.HasOpacity && mtl.Opacity < 0.99f))
            {
                double refractionIndex = 1.5;
                return new Dielectric(refractionIndex);
            }

            if (isExplicitMetal || (mtl.HasShininess && mtl.Shininess > 800.0f))
            {
                Color albedo = Color.WHITE;
                if (mtl.HasColorSpecular)
                {
                    albedo = new Color(mtl.ColorSpecular.R, mtl.ColorSpecular.G, mtl.ColorSpecular.B);
                }
                else if (mtl.HasColorDiffuse)
                {
                    albedo = new Color(mtl.ColorDiffuse.R, mtl.ColorDiffuse.G, mtl.ColorDiffuse.B);
                }

                double fuzz = 0.45;

                return new Metal(albedo, fuzz);
            }

            if (mtl.HasTextureDiffuse) {
                string textFilename = mtl.TextureDiffuse.FilePath;
                string textPath = Path.Combine(basePath, textFilename);
                Console.WriteLine($"Cargando textura: {textFilename}");

                var imageTexture = ImageLoader.Load(textPath);
                return new Diffuse(imageTexture);
            }
            if (mtl.HasColorDiffuse) {
                var color = mtl.ColorDiffuse;
                return new Diffuse(new Color(color.R, color.G, color.B));
            }

            Console.WriteLine($"[WARN] Material {mtl.Name} does not have texture or color. Using default");
            return def ?? new Diffuse(Color.PINK_HOT);
        }
    }
}
