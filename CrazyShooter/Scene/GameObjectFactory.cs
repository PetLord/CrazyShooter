using CrazyShooter.Rendering;
using Silk.NET.OpenGL;
using Shader = CrazyShooter.Rendering.Shader;

namespace CrazyShooter.Scene;

public static class GameObjectFactory
{
    public static Player CreatePlayer(GL gl, Shader shader)
    {
        string playerModelPath = "CrazyShooter.Assets.Textures.Duck.RubberDuck_LOD0.obj";
        string playerTexturePath = "CrazyShooter.Assets.Textures.Duck.Solid_yellow.png";
        uint textureId = ObjectLoader.LoadTexture(gl, playerTexturePath);
        var model = ObjectLoader.Load(playerModelPath, shader, textureId);
        var mesh = new Mesh(gl, model);
        var player = new Player(mesh);
        return player;
    }
    
}
