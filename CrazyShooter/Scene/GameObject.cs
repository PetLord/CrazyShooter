using CrazyShooter.Rendering;
using CrazyShooter.Tools;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace CrazyShooter.Scene;

public class GameObject : IDisposable
{
    private Vector3D<float> _position = Vector3D<float>.Zero;
    private Vector3D<float> _rotation = Vector3D<float>.Zero;
    private Vector3D<float> _scale = Vector3D<float>.One;

    public virtual Vector3D<float> Position
    {
        get => _position;
        set => _position = value;
    }

    public virtual Vector3D<float> Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    public virtual Vector3D<float> Scale
    {
        get => _scale;
        set => _scale = value;
    }
    public Mesh Mesh;

    public GameObject(Mesh mesh)
    {
        Mesh = mesh;
    }

    public virtual void Update(double deltaTime)
    {
        // Override in derived classes
    }

    public virtual unsafe void Render(Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        Matrix4X4<float> model =
            Matrix4X4.CreateScale(Scale) *
            Matrix4X4.CreateFromYawPitchRoll(
                MathUtils.ToRadians(Rotation.Y),
                MathUtils.ToRadians(Rotation.X),
                MathUtils.ToRadians(Rotation.Z)) *
            Matrix4X4.CreateTranslation(Position);

        Mesh.Render(model, view, projection);
    }

    public void Dispose()
    {
        Mesh.Dispose(); // Assuming Mesh implements IDisposable and releases VAO/VBO/etc.
    }
}
