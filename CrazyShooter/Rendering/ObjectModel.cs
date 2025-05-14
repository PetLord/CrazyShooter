namespace CrazyShooter.Rendering;

public class ObjectModel
{
    public float[] Vertices;       
    public uint[] Indices;
    public Shader Shader;      
    public uint TextureId;

    public ObjectModel(float[] vertices, uint[] indices, Shader shader, uint textureId)
    {
        Vertices = vertices;
        Indices = indices;
        Shader = shader;
        TextureId = textureId;
    }
}
