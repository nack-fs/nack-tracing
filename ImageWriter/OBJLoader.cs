using Assimp;
using NackEngine.core.space;
using NackEngine.objects;
using System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NackEngine.core.render;

namespace NackEngine.IO
{
    using static NackEngine.core.space.NVector;
    using Material = core.render.Material;
    using Point = NVector;

    public static class OBJLoader
    {
        public static HitCollection Load(string filepath, Material material) { 
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
                    || scene.RootNode == null) {
                    Console.WriteLine($"[ERROR] Fallo al procesar el OBJ: {filepath}");
                    return world;
                }

                int totalTriangles = 0;
                foreach (var mesh in scene.Meshes) {
                    Point[] vertices = new Point[mesh.VertexCount];

                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        vertices[i] = new Point(mesh.Vertices[i].X,
                                                mesh.Vertices[i].Y,
                                                mesh.Vertices[i].Z);
                    }

                    foreach (var face in mesh.Faces) {
                        if (face.IndexCount == 3) {
                            Point v0 = vertices[face.Indices[0]];
                            Point v1 = vertices[face.Indices[1]];
                            Point v2 = vertices[face.Indices[2]];

                            world.AddObject(new Triangle(v0, v1, v2, material));

                            totalTriangles++;
                        }
                    }
                }
                Console.WriteLine($"Modelo OBJ cargado correctamente. Total triángulos: {totalTriangles}");
            }
            catch (Exception e) {
                Console.WriteLine($"[ERROR] Cargando el OBJ: {e.Message}");
            }
            return world;
        }
    }
}
