using System.Globalization;
using System.Reflection;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StbImageSharp;


namespace CrazyShooter.Rendering;

public static class ObjectLoader
{
    public static ObjectModel Load(string path, Shader shader, uint textureId)
    {
        // Step 1: Read the raw data
        ReadObjectData(
            path,
            out var objVertices,
            out var objFaces,
            out var objFaceNormalIndices,
            out var objNormals,
            out var objFaceTextureIndices,
            out var objTextures
        );
        
        var interleaved = new List<float>();
        var indices = new List<uint>();
        var vertexMap = new Dictionary<string, uint>();
        uint currentIndex = 0;

        for (int f = 0; f < objFaces.Count; f++)
        {
            int[] face = objFaces[f];
            int[] normalIndices = f < objFaceNormalIndices.Count ? objFaceNormalIndices[f] :null;
            int[] texIndices = f < objFaceTextureIndices.Count ? objFaceTextureIndices[f] :null;

            for (int i = 0; i < face.Length; i++)
            {
                int vi = face[i];
                int? ni = (normalIndices != null && i < normalIndices.Length) ? normalIndices[i] : null;
                int? ti = (texIndices != null && i < texIndices.Length) ? texIndices[i] : null;

                float[] v = objVertices[vi];

                float[] n = ni.HasValue && ni.Value < objNormals.Count
                    ? objNormals[ni.Value]
                    : new float[]{ 0f, 1f, 0f };

                float[] t = ti.HasValue && ti.Value < objTextures.Count
                    ? objTextures[ti.Value]
                    : new float[] { 0f, 0f };

                string key = $"{vi}/{ti}/{ni}";
                if (!vertexMap.TryGetValue(key, out uint index))
                {
                    interleaved.AddRange(v); // pos
                    interleaved.AddRange(n); // normal
                    interleaved.AddRange(t); // texture coord

                    index = currentIndex++;
                    vertexMap[key] = index;
                }

                indices.Add(index);
            }
        }

        return new ObjectModel(interleaved.ToArray(), indices.ToArray(), shader, textureId);
    }
    
    public static (ObjectModel model, (Vector3D<float> min, Vector3D<float> max) bounds) LoadCollidable(string path, Shader shader, uint textureId)
    {
        // Step 1: Read the raw data
        ReadObjectData(
            path,
            out var objVertices,
            out var objFaces,
            out var objFaceNormalIndices,
            out var objNormals,
            out var objFaceTextureIndices,
            out var objTextures
        );
        
        var interleaved = new List<float>();
        var indices = new List<uint>();
        var vertexMap = new Dictionary<string, uint>();
        uint currentIndex = 0;

        for (int f = 0; f < objFaces.Count; f++)
        {
            int[] face = objFaces[f];
            int[] normalIndices = f < objFaceNormalIndices.Count ? objFaceNormalIndices[f] :null;
            int[] texIndices = f < objFaceTextureIndices.Count ? objFaceTextureIndices[f] :null;

            for (int i = 0; i < face.Length; i++)
            {
                int vi = face[i];
                int? ni = (normalIndices != null && i < normalIndices.Length) ? normalIndices[i] : null;
                int? ti = (texIndices != null && i < texIndices.Length) ? texIndices[i] : null;

                float[] v = objVertices[vi];

                float[] n = ni.HasValue && ni.Value < objNormals.Count
                    ? objNormals[ni.Value]
                    : new float[]{ 0f, 1f, 0f };

                float[] t = ti.HasValue && ti.Value < objTextures.Count
                    ? objTextures[ti.Value]
                    : new float[] { 0f, 0f };

                string key = $"{vi}/{ti}/{ni}";
                if (!vertexMap.TryGetValue(key, out uint index))
                {
                    interleaved.AddRange(v); // pos
                    interleaved.AddRange(n); // normal
                    interleaved.AddRange(t); // texture coord

                    index = currentIndex++;
                    vertexMap[key] = index;
                }

                indices.Add(index);
            }
        }
        
        return (
            new ObjectModel(interleaved.ToArray(), indices.ToArray(), shader, textureId),
            Tools.CollisionTools.ComputeBounds(objVertices)
        );
    }
    
    
    private static void ReadObjectData(
        string path,
        out List<float[]> objVertices,
        out List<int[]> objFaces,
        out List<int[]> objFaceNormalIndices,
        out List<float[]> objNormals,
        out List<int[]> objFaceTextureIndices,
        out List<float[]> objTextures)
    {
        // Check if the path is valid
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        // Initialize all output lists
        objVertices = new List<float[]>();
        objFaces = new List<int[]>();
        objNormals = new List<float[]>();
        objFaceNormalIndices = new List<int[]>();
        objTextures = new List<float[]>();
        objFaceTextureIndices = new List<int[]>();

        // Load the file stream
        using Stream objStream = File.OpenRead(path);

        if (objStream == null)
            throw new FileNotFoundException($"Could not open stream for: {path}");

        using StreamReader objReader = new StreamReader(objStream);

        while (!objReader.EndOfStream)
        {
            var line = objReader.ReadLine()?.Trim();

            // Skip empty lines or comments
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length < 2)
                continue;

            string prefix = tokens[0];
            string[] data = tokens.Skip(1).ToArray();

            switch (prefix)
            {
                case "v": // vertex position
                    objVertices.Add(data.Select(d => float.Parse(d, CultureInfo.InvariantCulture)).ToArray());
                    break;

                case "vn": // normal
                    objNormals.Add(data.Select(d => float.Parse(d, CultureInfo.InvariantCulture)).ToArray());
                    break;

                case "vt": // texture coordinate
                    objTextures.Add(data.Select(d => float.Parse(d, CultureInfo.InvariantCulture)).ToArray());
                    break;

                case "f":
                    var faceVertices = new List<int>();
                    var normalIndices = new List<int>();
                    var texIndices = new List<int>();

                    foreach (string part in data)
                    {
                        string[] split = part.Split('/');

                        // Handle vertex indices (required)
                        if (split.Length > 0 && int.TryParse(split[0], out int vIndex))
                        {
                            faceVertices.Add(vIndex - 1); // OBJ indices are 1-based
                        }
                        else
                        {
                            continue; // Skip invalid face vertex
                        }

                        // Handle texture indices (optional)
                        if (split.Length > 1)
                        {
                            if (int.TryParse(split[1], out int tIndex))
                            {
                                texIndices.Add(tIndex - 1);
                            }
                            else
                            {
                                // Add a placeholder if texture index is missing but might be needed for other vertices
                                texIndices.Add(-1);
                            }
                        }

                        // Handle normal indices (optional)
                        if (split.Length > 2)
                        {
                            if (int.TryParse(split[2], out int nIndex))
                            {
                                normalIndices.Add(nIndex - 1);
                            }
                            else
                            {
                                // Add a placeholder if normal index is missing but might be needed for other vertices
                                normalIndices.Add(-1);
                            }
                        }
                    }

                    // Only process if we have at least 3 vertices for a triangle
                    if (faceVertices.Count >= 3)
                    {
                        // Triangulate the face using fan triangulation
                        for (int i = 1; i < faceVertices.Count - 1; i++)
                        {
                            var triangleVerts = new int[] { faceVertices[0], faceVertices[i], faceVertices[i + 1] };
                            objFaces.Add(triangleVerts);

                            // Handle normal indices if available
                            if (normalIndices.Count > 0)
                            {
                                var normalIndicesArray = new int[3];
                                for (int j = 0; j < 3; j++)
                                {
                                    // Map from face vertices to corresponding normal indices
                                    int idx = j == 0 ? 0 : (j == 1 ? i : i + 1);
                                    normalIndicesArray[j] = idx < normalIndices.Count ? normalIndices[idx] : -1;
                                }
                                objFaceNormalIndices.Add(normalIndicesArray);
                            }

                            // Handle texture indices if available
                            if (texIndices.Count > 0)
                            {
                                var texIndicesArray = new int[3];
                                for (int j = 0; j < 3; j++)
                                {
                                    // Map from face vertices to corresponding texture indices
                                    int idx = j == 0 ? 0 : (j == 1 ? i : i + 1);
                                    texIndicesArray[j] = idx < texIndices.Count ? texIndices[idx] : -1;
                                }
                                objFaceTextureIndices.Add(texIndicesArray);
                            }
                        }
                    }
                    break;
            }
        }
    }
    
    public static unsafe uint LoadTexture(GL gl, string filePath)
    {
        using Image<Rgba32> image = Image.Load<Rgba32>(filePath);

        // Allocate buffer for pixel data in RGBA order
        var pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        fixed (byte* pixelPtr = pixels)
        {
            uint texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                (uint)image.Width, (uint)image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, pixelPtr);

            gl.GenerateMipmap(TextureTarget.Texture2D);

            // Set texture parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            return texture;
        }
    }
}