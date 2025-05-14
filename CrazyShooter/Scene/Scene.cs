using CrazyShooter.Rendering;
using Silk.NET.OpenGL;

namespace CrazyShooter.Scene;

public class Scene : IDisposable
{
    private Camera _camera { get; } = new();
    private Player _player { get; set; }
    private List<GameObject> gameObjects = new List<GameObject>();
    public float AspectRatio { get;  set; } = 16 / 9f;

    public void Update(double deltaTime)
    {
        _camera.Follow(_player.Position, _player.Rotation.X, _player.Rotation.Y);
        _player.Update(deltaTime);
        foreach (var t in gameObjects)
        {
            t.Update(deltaTime);
        }
    }
    
    public void Render(GL gl)
    {
        var viewMatrix = _camera.GetViewMatrix();
        var projectionMatrix = _camera.GetProjectionMatrix(AspectRatio);
        _player.Render(viewMatrix, projectionMatrix);
        foreach (var t in gameObjects)
        {
            t.Render(viewMatrix, projectionMatrix);
        }
    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
    }

    public void Dispose()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.Dispose();
        }
        _player.Dispose();
        
    }
}