using Assimp;
using NackEngine.core.render;
using NackEngine.core.render.materials;
using NackEngine.core.space;
using NackEngine.objects;

namespace NackEngine.IO.loaders
{
    using Material = core.render.Material;
    using Point = NVector;

    public static class OBJLoader
    {
        public static HitCollection Load(string filepath, Material defaultMaterial = null)
        {
            string actualPath = AssetConfig.GetModelPath(filepath);
            if (string.IsNullOrEmpty(actualPath)) { actualPath = filepath; }

            HitCollection world = new HitCollection();
            AssimpContext importer = new AssimpContext();

            try
            {
                Logger.Log($"[INFO] Loading OBJ model: {actualPath} ...");

                Scene scene = importer.ImportFile(actualPath,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateSmoothNormals |
                    PostProcessSteps.JoinIdenticalVertices
                );

                if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete)
                    || scene.RootNode == null)
                {
                    Logger.Log($"[ERROR] Error processing the OBJ file in {actualPath}");
                    return world;
                }

                string basePath = Path.GetDirectoryName(actualPath);
                List<Material> materials = new List<Material>();

                if (scene.HasMaterials)
                {
                    foreach (var mat in scene.Materials)
                    {
                        materials.Add(ConvertMaterial(mat, basePath, defaultMaterial));
                    }
                }

                int totalTriangles = 0;
                foreach (var mesh in scene.Meshes)
                {
                    Material actualMaterial = defaultMaterial;
                    if (mesh.MaterialIndex >= 0 && mesh.MaterialIndex < materials.Count)
                    {
                        actualMaterial = materials[mesh.MaterialIndex] ?? defaultMaterial;
                    }

                    Point[] vertices = new Point[mesh.VertexCount];
                    NVector[] UVs = new NVector[mesh.VertexCount];
                    NVector[] normals = new NVector[mesh.VertexCount];

                    bool hasUVs = mesh.HasTextureCoords(0);
                    bool hasNormals = mesh.HasNormals;

                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        vertices[i] = new Point(mesh.Vertices[i].X,
                                                mesh.Vertices[i].Y,
                                                mesh.Vertices[i].Z);

                        if (hasNormals)
                        {
                            normals[i] = new NVector(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                        }
                        else {
                            normals[i] = new NVector(0, 1, 0);
                        }

                        if (hasUVs)
                        {
                            var texCoordinate = mesh.TextureCoordinateChannels[0][i];
                            UVs[i] = new NVector(texCoordinate.X, texCoordinate.Y, 0);
                        }
                        else
                        {
                            UVs[i] = new NVector(0, 0, 0);
                        }
                    }

                    foreach (var face in mesh.Faces)
                    {
                        if (face.IndexCount == 3)
                        {
                            int i0 = face.Indices[0];
                            int i1 = face.Indices[1];
                            int i2 = face.Indices[2];

                            Point v0 = vertices[i0];
                            Point v1 = vertices[i1];
                            Point v2 = vertices[i2];

                            NVector uv0 = UVs[i0];
                            NVector uv1 = UVs[i1];
                            NVector uv2 = UVs[i2];

                            NVector n0 = normals[i0];
                            NVector n1 = normals[i1];
                            NVector n2 = normals[i2];

                            world.AddObject(new Triangle(v0, v1, v2, uv0, uv1, uv2, n0, n1, n2, actualMaterial));

                            totalTriangles++;
                        }
                    }
                }
                Logger.Log("[INFO] Modelo OBJ cargado correctamente.");
                Logger.Log($"[INFO] --- Total triángulos: {totalTriangles} ---");

                Logger.Log("[INFO] Material MTL cargado correctamente.");
                Logger.Log($"[INFO] --- Total materiales: {materials.Count} ---");
            }
            catch (Exception e)
            {
                Logger.Log($"[ERROR] Cargando el OBJ: {e.Message}, path: {actualPath}");
            }
            return world;
        }

        private static Material ConvertMaterial(Assimp.Material mtl, string basePath, Material def)
        {
            string matName = mtl.Name.ToLower();
            bool isExplicitMetal = matName.Contains("metal");
            bool isExplicitGlass = matName.Contains("glass");

            if (isExplicitGlass || (mtl.HasOpacity && mtl.Opacity < 0.99f))
            {
                float refractionIndex = 1.5f;

                if (mtl.HasReflectivity && mtl.Reflectivity > 1.0f) {
                    refractionIndex = mtl.Reflectivity;
                }

                Color glassColor = Color.WHITE;
                if (mtl.HasColorTransparent
                    && (mtl.ColorTransparent.R > 0 || mtl.ColorTransparent.G > 0 || mtl.ColorTransparent.B > 0))
                {
                    glassColor = new Color(mtl.ColorTransparent.R, mtl.ColorTransparent.G, mtl.ColorTransparent.B);
                }
                else if (mtl.HasColorDiffuse
                    && (mtl.ColorDiffuse.R < 1 || mtl.ColorDiffuse.G < 1 || mtl.ColorDiffuse.B < 1)) {
                    glassColor = new Color(mtl.ColorDiffuse.R, mtl.ColorDiffuse.G, mtl.ColorDiffuse.B);
                }
                return new Dielectric(refractionIndex, glassColor);
            }

            if (isExplicitMetal)
            {
                Color albedo = Color.WHITE;

                if (mtl.HasColorDiffuse)
                {
                    albedo = new Color(mtl.ColorDiffuse.R, mtl.ColorDiffuse.G, mtl.ColorDiffuse.B);
                }
                else if (mtl.HasColorSpecular)
                {
                    albedo = new Color(mtl.ColorSpecular.R, mtl.ColorSpecular.G, mtl.ColorSpecular.B);
                }
                float fuzz = 0.45f;
                return new Metal(albedo, fuzz);
            }

            float roughness = mtl.HasShininess ? Math.Clamp(1.0f - (mtl.Shininess / 1000.0f), 0.0f, 1.0f) : 1.0f;

            float specular = 0f;

            if (mtl.HasColorSpecular)
            {
                specular = (mtl.ColorSpecular.R + mtl.ColorSpecular.G + mtl.ColorSpecular.B) / 3.0f;
                float reflectIndex = 0.08f;
                specular = Math.Clamp(specular * reflectIndex, 0.0f, 0.05f);
            }

            if (mtl.HasTextureDiffuse)
            {
                string textFilename = mtl.TextureDiffuse.FilePath;
                string textPath = Path.Combine(basePath, textFilename);

                var imageTexture = ImageLoader.Load(textPath);
                return new Diffuse(imageTexture, specular, roughness);
            }

            if (mtl.HasColorDiffuse)
            {
                var color = mtl.ColorDiffuse;

                if (color.R == 0 && color.G == 0 && color.B == 0 && mtl.HasColorAmbient)
                {
                    color = mtl.ColorAmbient;
                }

                return new Diffuse(new Color(color.R, color.G, color.B), specular, roughness);
            }
            return def ?? new Diffuse(Color.PINK_HOT);

        }
    }
}
