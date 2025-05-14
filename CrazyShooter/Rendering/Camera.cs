using System.Numerics;
using CrazyShooter.Tools;
namespace CrazyShooter.Rendering;

public class Camera
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }
    
    public Vector3 Position { get; set; } = new(0f, 0f, 3f);
    public CameraMode Mode { get; set; } = CameraMode.ThirdPerson;
    public float Distance { get; set; } = 5.0f;
    private float Yaw { get; set; } = -90f;
    private float Pitch { get; set; }= 0f;
    private float Speed { get; set; } = 5f;
    private float Sensitivity { get; set; } = 0.1f;
    private float Zoom { get; set; } = 45f;

    private Vector3 Front { get; set; } = -Vector3.UnitZ;
    private Vector3 Up { get; set; } = Vector3.UnitY;
    private Vector3 Right { get; set; } = Vector3.UnitX;
    private Vector3 WorldUp { get; set; } = Vector3.UnitY;

    public Camera()
    {
        UpdateCameraVectors();
    }

    public Matrix4x4 GetViewMatrix()
        => Matrix4x4.CreateLookAt(Position, Position + Front, Up);

    public Matrix4x4 GetProjectionMatrix(float aspectRatio)
        => Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.ToRadians(Zoom), aspectRatio, 0.1f, 100f);

    public void ProcessKeyboard(Vector3 direction, float deltaTime)
    {
        float velocity = Speed * deltaTime;
        Position += direction * velocity;
    }

    public void ProcessMouseMovement(float deltaX, float deltaY, bool constrainPitch = true)
    {
        Yaw += deltaX * Sensitivity;
        Pitch -= deltaY * Sensitivity;

        if (constrainPitch)
        {
            Pitch = Math.Clamp(Pitch, -89f, 89f);
        }

        UpdateCameraVectors();
    }

    private void UpdateCameraVectors()
    {
        Vector3 front;
        front.X = MathF.Cos(MathUtils.ToRadians(Yaw)) * MathF.Cos(MathUtils.ToRadians(Pitch));
        front.Y = MathF.Sin(MathUtils.ToRadians(Pitch));
        front.Z = MathF.Sin(MathUtils.ToRadians(Yaw)) * MathF.Cos(MathUtils.ToRadians(Pitch));
        Front = Vector3.Normalize(front);
        Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }

    public void Follow(Vector3 targetPosition, float yaw, float pitch)
    {
        Yaw = yaw;
        Pitch = pitch;
        UpdateCameraVectors();

        if (Mode == CameraMode.FirstPerson)
        {
            Position = targetPosition;
        }
        else if (Mode == CameraMode.ThirdPerson)
        {
            Vector3 offset = -Front * Distance + new Vector3(0, Distance / 2, 0);
            Position = targetPosition + offset;
        }
    }

    public void ToggleView()
    {
        switch (Mode)
        {
            case CameraMode.FirstPerson:
                Mode = CameraMode.ThirdPerson;
                return;
            case CameraMode.ThirdPerson:
                Mode = CameraMode.FirstPerson;
                return;
        }
        
    }

}

