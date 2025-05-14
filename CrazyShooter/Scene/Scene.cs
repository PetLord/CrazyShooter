using Silk.NET.OpenGL;

namespace CrazyShooter.Scene;

public class Scene
{
    private List<GameObject> gameObjects = new List<GameObject>();
    
    public void Update(double deltaTime)
    {
        foreach (var t in gameObjects)
        {
            t.Update(deltaTime);
        }
    }
    
    public void Render(GL gl)
    {
        foreach (var t in gameObjects)
        {
            t.Render();
        }
    }
    
    
}