using CrazyShooter.Collision;
using CrazyShooter.Rendering;
using CrazyShooter.Tools;
using Silk.NET.Maths;

namespace CrazyShooter.Scene;

public class Player : CollidableObject
{
    public float MovementSpeed { get; set; } = 5.0f;
    public Player(Mesh mesh, Vector3D<float> meshMinBounds, Vector3D<float> meshMaxBounds) : base(mesh, meshMinBounds, meshMaxBounds) { }
    
    public virtual unsafe void Render(Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        Matrix4X4<float> model =
            Matrix4X4.CreateScale(Scale) *
            Matrix4X4.CreateRotationY(MathUtils.ToRadians(Rotation.Y)) *
            Matrix4X4.CreateTranslation(Position);

        
        Mesh.Render(model, view, projection);
    }
}
