using Silk.NET.Maths;
using CrazyShooter.Scene;
using Silk.NET.Input;

namespace CrazyShooter.Input;

public class PlayerInputHandler
{
    private readonly IKeyboard _keyboard;
    private readonly IMouse _mouse;
    
    private float yaw = -90f;
    private float pitch = 0f;
    private float sensitivity = 0.1f;
    
    private Vector2D<float> lastMousePos;
    private bool firstFrame = true;
    
    public Vector2D<float> MouseDelta { get; private set; }

    public PlayerInputHandler(IKeyboard keyboard, IMouse mouse)
    {
        _keyboard = keyboard;
        _mouse = mouse;
    }

    public void ProcessInput(Player player, double deltaTime)
    {
        float dt = (float)deltaTime;
        float speed = player.MovementSpeed;

        // Keyboard Movement
        if (_keyboard.IsKeyPressed(Key.W)) player.Position.Z -= speed * dt;
        if (_keyboard.IsKeyPressed(Key.S)) player.Position.Z += speed * dt;
        if (_keyboard.IsKeyPressed(Key.A)) player.Position.X -= speed * dt;
        if (_keyboard.IsKeyPressed(Key.D)) player.Position.X += speed * dt;

        // Mouse Movement
        Vector2D<float> mousePos = new Vector2D<float>(_mouse.Position.X, _mouse.Position.Y);

        if (firstFrame)
        {
            lastMousePos = mousePos;
            firstFrame = false;
        }

        MouseDelta = mousePos - lastMousePos;
        lastMousePos = mousePos;

        yaw += MouseDelta.X * sensitivity;
        pitch -= MouseDelta.Y * sensitivity;
        pitch = Math.Clamp(pitch, -89f, 89f);
        
        player.Rotation = new Vector3D<float>(pitch, yaw, 0);
    }
}