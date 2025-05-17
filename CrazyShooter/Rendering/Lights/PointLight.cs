using Silk.NET.Maths;

namespace CrazyShooter.Rendering.Lights;

public class PointLight : Light
{
    public Vector3D<float>Position { get; set; } = new(1.0f, 1.0f, 1.0f);
    public float Constant { get; set; } = 1f;
    public float Linear { get; set; } = 0.09f;
    public float Quadratic { get; set; } = 0.032f;
}