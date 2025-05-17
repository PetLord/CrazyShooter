using Silk.NET.Maths;

namespace CrazyShooter.Rendering.Lights;

public class SpotLight
{
    public Vector3D<float> Position { get; set; } = new(0f, 0f, 0f);
    public Vector3D<float> Direction { get; set; } = new(0f, -1f, 0f);
    public float Cutoff { get; set; } = 12.5f;  
    public float OuterCutoff { get; set; } = 17.5f; 
}