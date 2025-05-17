using System.Reflection;
using CrazyShooter.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;
using CrazyShooter.Tools;
using Shader = CrazyShooter.Rendering.Shader;

namespace CrazyShooter.Scene;

public class SkyBox : GameObject
{
    private static string skyBoxPath = "Assets.Skybox.skybox_sand.png";
    private static string skyBoxPathNormal = "Assets.Skybox.skybox.png";

    public SkyBox(GL gl, Shader shader) : base(new Mesh(gl, CreateSkyBox(gl, shader)))
    {
        Scale = new Vector3D<float>(2000f, 2000f, 2000f);
    }

    public static unsafe ObjectModel CreateSkyBox(GL gl, Shader shader)
    {
        const float padX = 0.001f;
        const float padY = 0.001f;
        // counter clockwise is front facing
        // vx, vy, vz, nx, ny, nz, tu, tv
        float[] vertexData = new float[] {
            // top face
            -0.5f, 0.5f, 0.5f,  0f, -1f, 0f, 0.25f + padX, 0.0f     + padY,
            0.5f, 0.5f, 0.5f,  0f, -1f, 0f, 0.5f  - padX, 0.0f     + padY,
            0.5f, 0.5f, -0.5f, 0f, -1f, 0f, 0.5f  - padX, 0.333f   - padY,
            -0.5f, 0.5f, -0.5f, 0f, -1f, 0f, 0.25f + padX, 0.333f   - padY,

            // front face
            -0.5f, 0.5f, 0.5f,  0f, 0f, -1f, 1.0f  - padX, 0.333f   + padY,
            -0.5f,-0.5f, 0.5f,  0f, 0f, -1f, 1.0f  - padX, 0.666f   - padY,
            0.5f,-0.5f, 0.5f,  0f, 0f, -1f, 0.75f + padX, 0.666f   - padY,
            0.5f, 0.5f, 0.5f,  0f, 0f, -1f, 0.75f + padX, 0.333f   + padY,

            // left face
            -0.5f, 0.5f, 0.5f,  1f, 0f, 0f,  0.0f  + padX, 0.333f   + padY,
            -0.5f, 0.5f,-0.5f,  1f, 0f, 0f,  0.25f - padX, 0.333f   + padY,
            -0.5f,-0.5f,-0.5f,  1f, 0f, 0f,  0.25f - padX, 0.666f   - padY,
            -0.5f,-0.5f, 0.5f,  1f, 0f, 0f,  0.0f  + padX, 0.666f   - padY,

            // bottom face
            -0.5f,-0.5f, 0.5f,  0f, 1f, 0f,  0.25f + padX, 1.0f     - padY,
            0.5f,-0.5f, 0.5f,  0f, 1f, 0f,  0.5f  - padX, 1.0f     - padY,
            0.5f,-0.5f,-0.5f,  0f, 1f, 0f,  0.5f  - padX, 0.666f   + padY,
            -0.5f,-0.5f,-0.5f,  0f, 1f, 0f,  0.25f + padX, 0.666f   + padY,

            // back face
            0.5f, 0.5f,-0.5f,  0f, 0f, 1f,  0.5f  - padX, 0.333f   + padY,
            -0.5f, 0.5f,-0.5f,  0f, 0f, 1f,  0.25f + padX, 0.333f   + padY,
            -0.5f,-0.5f,-0.5f,  0f, 0f, 1f,  0.25f + padX, 0.666f   - padY,
            0.5f,-0.5f,-0.5f,  0f, 0f, 1f,  0.5f  - padX, 0.666f   - padY,

            // right face
            0.5f, 0.5f, 0.5f, -1f, 0f, 0f,  0.75f + padX, 0.333f   + padY,
            0.5f, 0.5f,-0.5f, -1f, 0f, 0f,  0.5f  - padX, 0.333f   + padY,
            0.5f,-0.5f,-0.5f, -1f, 0f, 0f,  0.5f  - padX, 0.666f   - padY,
            0.5f,-0.5f, 0.5f, -1f, 0f, 0f,  0.75f + padX, 0.666f   - padY,
        };


        uint[] indexArray = new uint[] {
            0, 2, 1,
            0, 3, 2,

            4, 6, 5,
            4, 7, 6,

            8, 10, 9,
            10, 8, 11,

            12, 13, 14,
            12, 14, 15,

            17, 19, 16,
            17, 18, 19,

            20, 21, 22,
            20, 22, 23
        };

        // Load the skybox texture
        uint textureId = LoadSkyboxTexture(gl, skyBoxPath);
    
        // Create interleaved vertex data directly for ObjectModel
        var interleavedVertices = new List<float>();
    
        // Process each triangle from the index array
        for (int i = 0; i < indexArray.Length; i++)
        {
            uint index = indexArray[i];
            int baseIndex = (int)(index * 8); // 8 components per vertex (pos[3], normal[3], tex[2])
        
            // Position (3 components)
            interleavedVertices.Add(vertexData[baseIndex]);
            interleavedVertices.Add(vertexData[baseIndex + 1]);
            interleavedVertices.Add(vertexData[baseIndex + 2]);
        
            // Normal (3 components)
            interleavedVertices.Add(vertexData[baseIndex + 3]);
            interleavedVertices.Add(vertexData[baseIndex + 4]);
            interleavedVertices.Add(vertexData[baseIndex + 5]);
        
            // Texture coordinates (2 components)
            interleavedVertices.Add(vertexData[baseIndex + 6]);
            interleavedVertices.Add(vertexData[baseIndex + 7]);
        }
    
        // Create sequential indices for the new vertex array
        uint[] sequentialIndices = new uint[indexArray.Length];
        for (uint i = 0; i < sequentialIndices.Length; i++)
        {
            sequentialIndices[i] = i;
        }
    
        return new ObjectModel(interleavedVertices.ToArray(), sequentialIndices, shader, textureId);
    }

    private static unsafe uint LoadSkyboxTexture(GL gl, string resourceRelativePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        ImageResult result;
        string resourceFullPath = assembly.GetName().Name + "." + resourceRelativePath;
        using (Stream skyboxStream = assembly.GetManifestResourceStream(resourceFullPath))
        {
            if (skyboxStream == null)
                throw new FileNotFoundException($"Skybox texture not found:" + resourceFullPath);
            
            result = ImageResult.FromStream(skyboxStream, ColorComponents.RedGreenBlueAlpha);
        }

        fixed (byte* pixelPtr = result.Data)
        {
            uint texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                (uint)result.Width, (uint)result.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, pixelPtr);

            gl.GenerateMipmap(TextureTarget.Texture2D);

            // Set texture parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            return texture;
        }
    }
    
    public void RenderSkybox(Matrix4X4<float> view, Matrix4X4<float> projection)
    {
        Matrix4X4<float> skyboxView = view;
        skyboxView.M41 = 0;
        skyboxView.M42 = 0;
        skyboxView.M43 = 0;

        Position = Vector3D<float>.Zero;

        Matrix4X4<float> model =
            Matrix4X4.CreateScale(Scale);

        Mesh.Render(model, skyboxView, projection);
    }

}