using System.Numerics;
using Silk.NET.OpenGL;

namespace CrazyShooter.Rendering;

public class ObjectModel
{
    public float[] Vertices;       
    public uint[] Indices;

    public Shader Shader;      
    public uint TextureID;     
}
