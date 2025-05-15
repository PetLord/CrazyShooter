using CrazyShooter.Rendering;
using Silk.NET.Maths;
using CrazyShooter.Scene;
using Silk.NET.Input;

namespace CrazyShooter.Input;

public class PlayerInputHandler
{
    public enum MouseState
    {
        PlayMode,
        FreeMode,
    }
    
    private List<IKeyboard> keyboards = new();
    private List<IMouse> mice = new();
    
    private float yaw = -90f;
    private float pitch = 0f;
    private float sensitivity = 0.1f;
    
    private Vector2D<float> lastMousePos;
    private bool firstFrame = true;
    private MouseState currentMode = MouseState.FreeMode;
    
    public Vector2D<float> MouseDelta { get; private set; }

    public PlayerInputHandler(IKeyboard[] keyboards, IMouse[] mice)
    {
        foreach (IKeyboard keyboard in keyboards)
        {
            this.keyboards.Add(keyboard);
        }

        foreach (IMouse mouse in mice)
        {
            this.mice.Add(mouse);
        }
            
    }

    public void ProcessInput(Player player, double deltaTime, Camera camera)
    {
        float dt = (float)deltaTime;
        float speed = player.MovementSpeed;

        // Aggregate keyboard input: if any keyboard has the key pressed, consider it pressed
        bool wPressed = keyboards.Any(kb => kb.IsKeyPressed(Key.W));
        bool sPressed = keyboards.Any(kb => kb.IsKeyPressed(Key.S));
        bool aPressed = keyboards.Any(kb => kb.IsKeyPressed(Key.A));
        bool dPressed = keyboards.Any(kb => kb.IsKeyPressed(Key.D));

        Vector3D<float> moveDirection = Vector3D<float>.Zero;
        
        if (wPressed) moveDirection += camera.Front;
        if (sPressed) moveDirection -= camera.Front;
        if (aPressed) moveDirection -= camera.Right;
        if (dPressed) moveDirection += camera.Right;
        
        if (moveDirection.LengthSquared > 0)
        {
            moveDirection = Vector3D.Normalize(moveDirection);
            player.Position += moveDirection * speed * dt;
        }
        
        if (mice.Count > 0)
        {
            var mouse = mice[0];
            Vector2D<float> mousePos = new(mouse.Position.X, mouse.Position.Y);

            if (firstFrame)
            {
                lastMousePos = mousePos;
                firstFrame = false;
            }

            Vector2D<float> mouseDelta = mousePos - lastMousePos;
            lastMousePos = mousePos;

            // camera.ProcessMouseMovement(mouseDelta.X, mouseDelta.Y);
            player.Rotation = new Vector3D<float>(
                Math.Clamp(player.Rotation.X - mouseDelta.Y * sensitivity, -89f, 89f),
                player.Rotation.Y + mouseDelta.X * sensitivity,                       
                0);
        }
    }
    
    public void SetMouseMode(MouseState mouseState)
    {
        currentMode = mouseState;
        switch (mouseState)
        {
            case MouseState.PlayMode:
                SwitchToPlay();
                break;
            case MouseState.FreeMode:
                SwitchToFreeMode();
                break;
        }
    }

    private void SwitchToPlay()
    {
        IMouse currentMouse = mice[0];
        currentMode = MouseState.PlayMode;
        currentMouse.Cursor.CursorMode = CursorMode.Raw;
        currentMouse.Cursor.IsConfined = true;
    }

    private void SwitchToFreeMode()
    {
        IMouse currentMouse = mice[0];
        currentMode = MouseState.FreeMode;
        currentMouse.Cursor.CursorMode = CursorMode.Normal;
        currentMouse.Cursor.IsConfined = false;

    }
    
}