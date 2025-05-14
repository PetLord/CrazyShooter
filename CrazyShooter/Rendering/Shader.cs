using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace CrazyShooter.Rendering;

public class Shader : IDisposable
{
    private readonly GL _gl;
    public uint Handle { get; private set; }

    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;

        uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

        Handle = _gl.CreateProgram();
        _gl.AttachShader(Handle, vertexShader);
        _gl.AttachShader(Handle, fragmentShader);
        _gl.LinkProgram(Handle);

        // Check for linking errors
        _gl.GetProgram(Handle, GLEnum.LinkStatus, out var status);
        if (status == 0)
            throw new Exception($"Program link error: {_gl.GetProgramInfoLog(Handle)}");

        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);
        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var code);
        if (code != (int)GLEnum.True)
            throw new Exception($"Error compiling {type}: {_gl.GetShaderInfoLog(shader)}");
        return shader;
    }

    public void Use()
    {
        _gl.UseProgram(Handle);
    }

    public int GetAttribLocation(string name)
    {
        return _gl.GetAttribLocation(Handle, name);
    }

    public int GetUniformLocation(string name)
    {
        return _gl.GetUniformLocation(Handle, name);
    }


    public unsafe void SetMatrix4(string name, Matrix4X4<float> matrix)
    {
        int location = GetUniformLocation(name);
        _gl.UniformMatrix4(location, 1, false, (float*)&matrix);
    
    }

    
    public void Dispose()
    {
        _gl.DeleteProgram(Handle);
    }

}
