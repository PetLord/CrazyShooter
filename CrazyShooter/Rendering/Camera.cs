using Silk.NET.Maths;
using CrazyShooter.Tools;

namespace CrazyShooter.Rendering;

public class Camera
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }

    public Vector3D<float> Position { get; set; } = new(0f, 0f, 3f);
    public CameraMode Mode { get; set; } = CameraMode.ThirdPerson;
    public float Distance { get; set; } = 5.0f;
    private float Near { get; set; } = 0.1f;
    private float Far { get; set; } = 100f;
    private float Yaw { get; set; } = -90f;
    private float Pitch { get; set; } = 0f;
    private float Speed { get; set; } = 5f;
    private float Sensitivity { get; set; } = 0.1f;
    private float Zoom { get; set; } = 45f;

    private Vector3D<float> Front { get; set; } = -Vector3D<float>.UnitZ;
    private Vector3D<float> Up { get; set; } = Vector3D<float>.UnitY;
    private Vector3D<float> Right { get; set; } = Vector3D<float>.UnitX;
    private Vector3D<float> WorldUp { get; set; } = Vector3D<float>.UnitY;

    public Camera()
    {
        UpdateCameraVectors();
    }

    public Matrix4X4<float> GetViewMatrix()
    {
        return Matrix4X4.CreateLookAt(Position, Position + Front, Up);
    }

    public Matrix4X4<float> GetProjectionMatrix(float aspectRatio)
    {
        return Matrix4X4.CreatePerspectiveFieldOfView(
            MathUtils.ToRadians(Zoom), aspectRatio, Near, Far);
    }
        

    public void ProcessKeyboard(Vector3D<float> direction, float deltaTime)
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
        float yawRad = MathUtils.ToRadians(Yaw);
        float pitchRad = MathUtils.ToRadians(Pitch);

        Vector3D<float> front = new(
            MathF.Cos(yawRad) * MathF.Cos(pitchRad),
            MathF.Sin(pitchRad),
            MathF.Sin(yawRad) * MathF.Cos(pitchRad)
        );

        Front = Vector3D.Normalize(front);
        Right = Vector3D.Normalize(Vector3D.Cross(Front, WorldUp));
        Up = Vector3D.Normalize(Vector3D.Cross(Right, Front));
    }

    public void Follow(Vector3D<float> targetPosition, float yaw, float pitch)
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
            Vector3D<float> offset = -Front * Distance + new Vector3D<float>(0, Distance / 2, 0);
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
            default:
                return;
        }
        
    }
}
