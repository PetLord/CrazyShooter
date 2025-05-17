using CrazyShooter.Rendering;

namespace CrazyShooter.Scene;

public class Player : GameObject
{
    public float MovementSpeed { get; set; } = 2.0f;
    public Player(Mesh mesh) : base(mesh) { }
    
}
