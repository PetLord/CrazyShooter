using Silk.NET.Maths;

namespace CrazyShooter.Rendering.Lights;

public class Light
{
    public Vector3D<float>Ambient { get; set; } = new(0.2f, 0.2f, 0.2f);
    public Vector3D<float>Diffuse { get; set; } = new(0.5f, 0.5f, 0.5f);
    public Vector3D<float>Specular { get; set; } = new(1.0f, 1.0f, 1.0f);
}