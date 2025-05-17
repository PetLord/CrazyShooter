using CrazyShooter.Collision;
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
        var (model, (min, max)) = ObjectLoader.LoadCollidable(Assets.Models.Duck, shader, textureId);
        var mesh = new Mesh(gl, model);
        var player = new Player(mesh, min, max);
        player.Scale = new Vector3D<float>(0.25f, 0.25f, 0.25f);
        player.Rotation = new Vector3D<float>(0f, 0f, 0f);
        return player;
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
        floor.Position = new Vector3D<float>(0f, 0f, 0f);
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

    public static CollidableObject CreateCollidableGameObject(GL gl, Shader shader, string resourceModelPath, string resourceTexturePath)
    {
        uint textureId = ObjectLoader.LoadTexture(gl, resourceTexturePath);
        var (model, bounds) = ObjectLoader.LoadCollidable(resourceModelPath, shader, textureId);
        var mesh = new Mesh(gl, model);
        var obj = new CollidableObject(mesh, bounds.min, bounds.max);
        return obj;
    }

    public static CompositeGameObject LoadPalm1(GL gl, Shader shader)
    {
        uint textureId = ObjectLoader.LoadTexture(gl, Assets.Textures.Palm1);
        var (bark1, bark1Bounds) = ObjectLoader.LoadCollidable(Assets.Models.Palm1_Bark1, shader, textureId);
        var bark1Mesh = new Mesh(gl, bark1);
        var bark1Obj = new CompositeGameObject(bark1Mesh, bark1Bounds.min, bark1Bounds.max);
        
        var (bark2, bark2Bounds) = ObjectLoader.LoadCollidable(Assets.Models.Palm1_Bark2, shader, textureId);
        var bark2Mesh = new Mesh(gl, bark2);
        var bark2Obj = new CollidableObject(bark2Mesh, bark2Bounds.min, bark2Bounds.max);
        
        var (leaves, leavesBounds) = ObjectLoader.LoadCollidable(Assets.Models.Palm1_Leaves, shader, textureId);
        var leavesMesh = new Mesh(gl, leaves);
        var leavesObj = new CollidableObject(leavesMesh, leavesBounds.min, leavesBounds.max);
        
        var (vines, vinesBounds) = ObjectLoader.LoadCollidable(Assets.Models.Palm1_Vines, shader, textureId);
        var vinesMesh = new Mesh(gl, vines);
        var vinesObj = new CollidableObject(vinesMesh, vinesBounds.min, vinesBounds.max);
        
        bark1Obj.AddChild(bark2Obj);
        bark1Obj.AddChild(vinesObj);
        bark1Obj.AddChild(leavesObj);
        return bark1Obj;
    }
}
