using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace CrazyShooter.Rendering;

public class Mesh
{
    public uint Vao { get;}
    public uint Vbo { get;}
    public uint Ebo { get;}
    public int IndexCount { get; }

    private Shader Shader { get; }
    public uint TextureId { get; }

    public unsafe Mesh(GL gl, ObjectModel model)
    {
        Shader = model.Shader;
        TextureId = model.TextureId;
        IndexCount = model.Indices.Length;

        Vao = gl.GenVertexArray();
        Vbo = gl.GenBuffer();
        Ebo = gl.GenBuffer();

        gl.BindVertexArray(Vao);

        // Upload vertex data
        gl.BindBuffer(GLEnum.ArrayBuffer, Vbo);
        gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)(model.Vertices.AsSpan()), GLEnum.StaticDraw);

        // Upload index data
        gl.BindBuffer(GLEnum.ElementArrayBuffer, Ebo);
        gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)(model.Indices.AsSpan()), GLEnum.StaticDraw);

        int stride = 8 * sizeof(float); // 3 pos + 3 norm + 2 tex

        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)0);
        gl.EnableVertexAttribArray(0); // Position

        gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)(3 * sizeof(float)));
        gl.EnableVertexAttribArray(1); // Normal

        gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)(6 * sizeof(float)));
        gl.EnableVertexAttribArray(2); // Texture coordinate

        gl.BindVertexArray(0);
    }
    
    public unsafe void Render(GL gl, Matrix4X4<float> model, Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        Shader.Use();

        int modelLoc = gl.GetUniformLocation(Shader.Handle, "uModel");
        gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        int viewLoc = gl.GetUniformLocation(Shader.Handle, "uView");
        gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);

        int projLoc = gl.GetUniformLocation(Shader.Handle, "uProjection");
        gl.UniformMatrix4(projLoc, 1, false, (float*)&projection);

        if (TextureId != 0)
        {
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, TextureId);
            int texLoc = gl.GetUniformLocation(Shader.Handle, "uTexture");
            gl.Uniform1(texLoc, 0);
        }

        gl.BindVertexArray(Vao);
        gl.BindBuffer(GLEnum.ElementArrayBuffer, Ebo);
        gl.DrawElements(PrimitiveType.Triangles, (uint)IndexCount, DrawElementsType.UnsignedInt, null);
    }

}