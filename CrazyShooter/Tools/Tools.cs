using System.Reflection;
using Silk.NET.OpenGL;

namespace CrazyShooter.Tools
{
    public static class MathUtils
    {
        public static float ToRadians(float degrees) => degrees * (MathF.PI / 180f);
        public static float ToDegrees(float radians) => radians * (180f / MathF.PI);
    }

    public static class ShaderUtils
    {
        public static string GetEmbeddedResourceAsString(string resourceRelativePath)
        {
            string resourceFullPath = Assembly.GetExecutingAssembly().GetName().Name + "." + resourceRelativePath;

            var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFullPath);
            if (resStream == null)
                throw new FileNotFoundException(resourceRelativePath);
            var resStreamReader = new StreamReader(resStream);
            var text = resStreamReader.ReadToEnd();
            return text;
        }
    }

    public static class ProgramUtils
    {
        public static uint LinkProgram(GL gl)
        {
            uint vShader = gl.CreateShader(ShaderType.VertexShader);
            uint fShader = gl.CreateShader(ShaderType.FragmentShader);

            gl.ShaderSource(vShader, ShaderUtils.GetEmbeddedResourceAsString("Assets.Shaders.VertexShader.vert"));
            gl.CompileShader(vShader);
            gl.GetShader(vShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + gl.GetShaderInfoLog(vShader));

            gl.ShaderSource(fShader, ShaderUtils.GetEmbeddedResourceAsString("Assets.Shaders.FragmentShader.frag"));
            gl.CompileShader(fShader);

            uint program = gl.CreateProgram();
            gl.AttachShader(program, vShader);
            gl.AttachShader(program, fShader);
            gl.LinkProgram(program);
            gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {gl.GetProgramInfoLog(program)}");
            }
            gl.DetachShader(program, vShader);
            gl.DetachShader(program, fShader);
            gl.DeleteShader(vShader);
            gl.DeleteShader(fShader);
            return program;
        }
    }
}