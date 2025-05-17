using System.Numerics;
using System.Reflection;
using CrazyShooter.Rendering;
using Silk.NET.Maths;
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

    public static class LightUtils
    {
        private const string LightColorVariableName = "lightColor";
        private const string LightPositionVariableName = "lightPos";
        private const string ViewPosVariableName = "viewPos";
        private const string ShininessVariableName = "shininess";
        
        private static void SetLightColor(GL gl, uint program, Vector3D<float> color)
        {
            int location = gl.GetUniformLocation(program, LightColorVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightColorVariableName} uniform not found on shader.");
            }

            gl.Uniform3(location, color.X, color.Y, color.Z);
            CheckError(gl);
        }

        private static void SetLightPosition(GL gl, uint program, Vector3D<float> position)
        {
            int location = gl.GetUniformLocation(program, LightPositionVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightPositionVariableName} uniform not found on shader.");
            }

            gl.Uniform3(location, position.X, position.Y, position.Z);
            CheckError(gl);
        }
        
        private static void SetViewerPosition(GL gl, uint program, Camera camera)
        {
            int location = gl.GetUniformLocation(program, ViewPosVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewPosVariableName} uniform not found on shader.");
            }

            gl.Uniform3(location, camera.Position.X, camera.Position.Y, camera.Position.Z);
            CheckError(gl);
        }

        private static void SetShininess(GL gl, uint program, uint shininess)
        {
            int location = gl.GetUniformLocation(program, ShininessVariableName);

            if (location == -1)
            {
                throw new Exception($"{ShininessVariableName} uniform not found on shader.");
            }

            gl.Uniform1(location, shininess);
            CheckError(gl);
        }
        
        private static void CheckError(GL gl)
        {
            var error = (ErrorCode)gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }


    public static class CollisionTools
    {
        public static (Vector3D<float> min, Vector3D<float> max) ComputeBounds(List<float[]> objVertices)
        {
            if (objVertices.Count == 0)
                return (Vector3D<float>.Zero, Vector3D<float>.Zero);

            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

            foreach (var v in objVertices)
            {
                minX = MathF.Min(minX, v[0]);
                minY = MathF.Min(minY, v[1]);
                minZ = MathF.Min(minZ, v[2]);

                maxX = MathF.Max(maxX, v[0]);
                maxY = MathF.Max(maxY, v[1]);
                maxZ = MathF.Max(maxZ, v[2]);
            }

            var min = new Vector3D<float>(minX, minY, minZ);
            var max = new Vector3D<float>(maxX, maxY, maxZ);
            return (min, max);
        }
    }
}
