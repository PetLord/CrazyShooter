using System.Globalization;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

                case "f": // face (possibly with v/vt/vn)
                    var face = new List<int>();
                    var normalIndices = new List<int>();
                    var texIndices = new List<int>();

                    foreach (string part in data)
                    {
                        string[] split = part.Split('/');

                        // Indexing: OBJ is 1-based, so subtract 1
                        int vIndex = int.Parse(split[0]) - 1;
                        face.Add(vIndex);

                        if (split.Length > 1 && !string.IsNullOrWhiteSpace(split[1]))
                            texIndices.Add(int.Parse(split[1]) - 1);

                        if (split.Length > 2 && !string.IsNullOrWhiteSpace(split[2]))
                            normalIndices.Add(int.Parse(split[2]) - 1);
                    }

                    objFaces.Add(face.ToArray());

                    if (normalIndices.Count > 0)
                        objFaceNormalIndices.Add(normalIndices.ToArray());

                    if (texIndices.Count > 0)
                        objFaceTextureIndices.Add(texIndices.ToArray());

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