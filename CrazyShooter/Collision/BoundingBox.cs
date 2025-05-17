using Silk.NET.Maths;

namespace CrazyShooter.Collision;

public class BoundingBox
{
    public Vector3D<float> Min;
    public Vector3D<float> Max;

    public BoundingBox(Vector3D<float> min, Vector3D<float> max)
    {
        Min = min;
        Max = max;
    }

    public bool Intersects(BoundingBox other)
    {
        return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
               (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
               (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
    }
    
    public Vector3D<float> Center =>
        (Min + Max) / 2f;
}