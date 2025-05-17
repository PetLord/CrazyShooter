using CrazyShooter.Collision;
using CrazyShooter.Rendering;
using Silk.NET.Maths;

namespace CrazyShooter.Scene;

public class CompositeGameObject : CollidableObject
{
    private readonly List<GameObject> children = new();

    public CompositeGameObject(Mesh mesh, Vector3D<float> min, Vector3D<float> max) : base(mesh, min, max)
    {
    }

    public void AddChild(GameObject child)
    {
        children.Add(child);
    }

    public List<GameObject> GetChildren()
    {
        return children;
    }
    
    public void RemoveChild(GameObject child)
    {
        children.Remove(child);
    }

    public override void Update(double deltaTime)
    {
        base.Update(deltaTime);
        foreach (var child in children)
            child.Update(deltaTime);
    }

    public override void Render(Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        base.Render(view, projection);
        foreach (var child in children)
            child.Render(view, projection);
    }
    
    
    public override Vector3D<float> Position
    {
        get => base.Position;
        set
        {
            var delta = value - base.Position;
            base.Position = value;

            foreach (var child in children)
            {
                child.Position += delta;
            }
        }
    }
    
    public override Vector3D<float> Rotation
    {
        get => base.Rotation;
        set
        {
            var delta = value - base.Rotation;
            base.Rotation = value;

            foreach (var child in children)
            {
                child.Rotation += delta;
            }
        }
    }

}
