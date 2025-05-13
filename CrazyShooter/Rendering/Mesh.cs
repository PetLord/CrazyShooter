using Silk.NET.OpenGL;

namespace CrazyShooter.Rendering;

public class Mesh
{
    public uint Vao { get;}
    public uint Vbo { get;}
    public uint Ebo { get;}
    public int IndexCount { get; }

    private Shader Shader { get; }
    public uint TextureID { get; }

    public unsafe Mesh(GL gl, ObjectModel model)
    {
        Shader = model.Shader;
        TextureID = model.TextureID;
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
    
    public unsafe void Render(GL gl)
    {
        Shader.Use();
        gl.BindVertexArray(Vao);
        gl.DrawElements(PrimitiveType.Triangles, (uint)IndexCount, DrawElementsType.UnsignedInt, null);
    }
}