using Silk.NET.Maths;

namespace CrazyShooter.Rendering.Lights;

public class DirectionalLight : Light
{
    public Vector3D<float> Direction { get; set; } = new(0f, -1f, 0f);
}
