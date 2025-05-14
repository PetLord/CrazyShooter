using CrazyShooter.Tools;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace CrazyShooter.Rendering;

public class GameObject
{
    public Vector3D<float> Position = Vector3D<float>.Zero;
    public Vector3D<float> Rotation = Vector3D<float>.Zero;
    public Vector3D<float> Scale = Vector3D<float>.One;

    public Mesh Mesh;

    public GameObject(Mesh mesh)
    {
        Mesh = mesh;
    }

    public virtual void Update(double deltaTime)
    {
        // Override in derived classes
    }

    public unsafe void Render(GL gl, uint shaderProgram, Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        // Compute model matrix
        Matrix4X4<float> model =
            Matrix4X4.CreateScale(Scale) *
            Matrix4X4.CreateFromYawPitchRoll(
                MathUtils.ToRadians(Rotation.Y),
                MathUtils.ToRadians(Rotation.X),
                MathUtils.ToRadians(Rotation.Z)) *
            Matrix4X4.CreateTranslation(Position);

        // Send all matrices to the shader
        int modelLoc = gl.GetUniformLocation(shaderProgram, "uModel");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        int viewLoc = gl.GetUniformLocation(shaderProgram, "uView");
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);

        int projLoc = gl.GetUniformLocation(shaderProgram, "uProjection");
        gl.UniformMatrix4(projLoc, 1, false, (float*)&projection);

        // Optional: bind texture if needed
        if (Mesh.TextureId != 0)
        {
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, Mesh.TextureId);
        }

        // Draw the mesh
        gl.BindVertexArray(Mesh.Vao);
        gl.BindBuffer(GLEnum.ElementArrayBuffer, Mesh.Ebo);
        gl.DrawElements(PrimitiveType.Triangles, (uint)Mesh.IndexCount, DrawElementsType.UnsignedInt, null);
    }

}
