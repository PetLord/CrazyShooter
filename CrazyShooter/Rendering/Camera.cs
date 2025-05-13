using System.Numerics;

namespace CrazyShooter.Rendering
{
    public class Camera
    {
        private Vector3 Position { get; set; } = new(0f, 0f, 3f);
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
            => Matrix4x4.CreatePerspectiveFieldOfView(ToRadians(Zoom), aspectRatio, 0.1f, 100f);

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
            front.X = MathF.Cos(ToRadians(Yaw)) * MathF.Cos(ToRadians(Pitch));
            front.Y = MathF.Sin(ToRadians(Pitch));
            front.Z = MathF.Sin(ToRadians(Yaw)) * MathF.Cos(ToRadians(Pitch));
            Front = Vector3.Normalize(front);
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        private float ToRadians(float degrees) => degrees * (MathF.PI / 180f);

    }
}