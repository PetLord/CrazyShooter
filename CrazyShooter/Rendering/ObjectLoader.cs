using System.Globalization;

namespace CrazyShooter.Rendering;

public static class ObjLoader
{
    public static ObjectModel Load(string path)
    {
        var model = new ObjectModel();
        return model;
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

}