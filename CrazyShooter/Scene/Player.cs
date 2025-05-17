using CrazyShooter.Collision;
using CrazyShooter.Rendering;
using Silk.NET.Maths;

namespace CrazyShooter.Scene;

public class Player : CollidableObject
{
    public float MovementSpeed { get; set; } = 2.0f;
    public Player(Mesh mesh, Vector3D<float> meshMinBounds, Vector3D<float> meshMaxBounds) : base(mesh, meshMinBounds, meshMaxBounds) { }
}
