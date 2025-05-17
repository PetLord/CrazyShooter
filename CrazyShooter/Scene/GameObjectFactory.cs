using CrazyShooter.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = CrazyShooter.Rendering.Shader;

namespace CrazyShooter.Scene;

public static class GameObjectFactory
{
    public static Player CreatePlayer(GL gl, Shader shader)
    {
        uint textureId = ObjectLoader.LoadTexture(gl, Assets.Textures.Duck);
        var model = ObjectLoader.Load(Assets.Models.Duck, shader, textureId);
        var mesh = new Mesh(gl, model);
        var player = new Player(mesh);
        player.Scale = new Vector3D<float>(0.25f, 0.25f, 0.25f);
        player.Rotation = new Vector3D<float>(0, 0, 0);
        return player;
    }
    
    public static GameObject CreateCube(GL gl, Shader shader)
    {
        // Define cube vertices (position, normal, texcoords)
        float[] cubeVertices = {
            -0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  0f, 0f,
            0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  1f, 0f,
            0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  1f, 1f,
            0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  1f, 1f,
            -0.5f,  0.5f,  0.5f,  0f, 0f, 1f,  0f, 1f,
            -0.5f, -0.5f,  0.5f,  0f, 0f, 1f,  0f, 0f,

            -0.5f, -0.5f, -0.5f,  0f, 0f, -1f, 0f, 0f,
            -0.5f,  0.5f, -0.5f,  0f, 0f, -1f, 0f, 1f,
            0.5f,  0.5f, -0.5f,  0f, 0f, -1f, 1f, 1f,
            0.5f,  0.5f, -0.5f,  0f, 0f, -1f, 1f, 1f,
            0.5f, -0.5f, -0.5f,  0f, 0f, -1f, 1f, 0f,
            -0.5f, -0.5f, -0.5f,  0f, 0f, -1f, 0f, 0f,

            -0.5f,  0.5f,  0.5f,  -1f, 0f, 0f, 1f, 0f,
            -0.5f,  0.5f, -0.5f,  -1f, 0f, 0f, 1f, 1f,
            -0.5f, -0.5f, -0.5f,  -1f, 0f, 0f, 0f, 1f,
            -0.5f, -0.5f, -0.5f,  -1f, 0f, 0f, 0f, 1f,
            -0.5f, -0.5f,  0.5f,  -1f, 0f, 0f, 0f, 0f,
            -0.5f,  0.5f,  0.5f,  -1f, 0f, 0f, 1f, 0f,

            0.5f,  0.5f,  0.5f,  1f, 0f, 0f, 1f, 0f,
            0.5f, -0.5f, -0.5f,  1f, 0f, 0f, 0f, 1f,
            0.5f,  0.5f, -0.5f,  1f, 0f, 0f, 1f, 1f,
            0.5f, -0.5f, -0.5f,  1f, 0f, 0f, 0f, 1f,
            0.5f,  0.5f,  0.5f,  1f, 0f, 0f, 1f, 0f,
            0.5f, -0.5f,  0.5f,  1f, 0f, 0f, 0f, 0f,

            -0.5f,  0.5f, -0.5f,  0f, 1f, 0f, 0f, 1f,
            -0.5f,  0.5f,  0.5f,  0f, 1f, 0f, 0f, 0f,
            0.5f,  0.5f,  0.5f,  0f, 1f, 0f, 1f, 0f,
            0.5f,  0.5f,  0.5f,  0f, 1f, 0f, 1f, 0f,
            0.5f,  0.5f, -0.5f,  0f, 1f, 0f, 1f, 1f,
            -0.5f,  0.5f, -0.5f,  0f, 1f, 0f, 0f, 1f,

            -0.5f, -0.5f, -0.5f,  0f, -1f, 0f, 0f, 1f,
            0.5f, -0.5f,  0.5f,  0f, -1f, 0f, 1f, 0f,
            -0.5f, -0.5f,  0.5f,  0f, -1f, 0f, 0f, 0f,
            0.5f, -0.5f,  0.5f,  0f, -1f, 0f, 1f, 0f,
            -0.5f, -0.5f, -0.5f,  0f, -1f, 0f, 0f, 1f,
            0.5f, -0.5f, -0.5f,  0f, -1f, 0f, 1f, 1f,
        };

        uint[] indices = new uint[0];
        var model = new ObjectModel(cubeVertices, indices, shader, 0);
        var mesh = new Mesh(gl, model);
        var cube = new GameObject(mesh);
        cube.Scale = new Vector3D<float>(5f, 5f, 5f);
        return cube;
    }
    
    public static GameObject CreateFloor(GL gl, Shader shader)
    {
        // 2 triangles forming a large square in XZ plane (the floor)
        float tileRepeat = 10f;

        float[] floorVertices = {
            -10f, 0f, -10f,   0f, 1f, 0f,   0f, 0f,
            10f, 0f, -10f,   0f, 1f, 0f,   tileRepeat, 0f,
            10f, 0f,  10f,   0f, 1f, 0f,   tileRepeat, tileRepeat,
            -10f, 0f,  10f,   0f, 1f, 0f,   0f, tileRepeat,
        };


        uint[] indices = {
            0, 1, 2,
            0, 2, 3
        };
        
        uint textureId = ObjectLoader.LoadTexture(gl, Assets.Textures.Sand);
        var model = new ObjectModel(floorVertices, indices, shader, textureId);
        var mesh = new Mesh(gl, model);
        var floor = new GameObject(mesh);
        floor.Position = new Silk.NET.Maths.Vector3D<float>(0f, 0f, 0f);
        floor.Scale = new Vector3D<float>(10f, 10f, 10f);
        return floor;
    }

    public static GameObject CreateGameObject(GL gl, Shader shader, string resourceModelPath, string resourceTexturePath)
    {
        uint textureId = ObjectLoader.LoadTexture(gl, resourceTexturePath);
        var model = ObjectLoader.Load(resourceModelPath, shader, textureId);
        var mesh = new Mesh(gl, model);
        var obj = new GameObject(mesh);
        return obj;
    }

}
