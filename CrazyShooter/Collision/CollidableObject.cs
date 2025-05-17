using CrazyShooter.Rendering;
using CrazyShooter.Scene;
using Silk.NET.Maths;

namespace CrazyShooter.Collision;

public class CollidableObject : GameObject, ICollidable
{
    public Vector3D<float> MeshMinBounds;
    public Vector3D<float> MeshMaxBounds;
    
    public CollidableObject(Mesh mesh, Vector3D<float> meshMinBounds, Vector3D<float> meshMaxBounds) : base(mesh)
    {
        this.MeshMinBounds = meshMinBounds;
        this.MeshMaxBounds = meshMaxBounds;
    }
    
    public BoundingBox BoundingBox =>
        new(
            Position + MeshMinBounds * Scale,
            Position + MeshMaxBounds * Scale
        );

}