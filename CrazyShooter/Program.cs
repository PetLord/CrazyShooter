using CrazyShooter.Rendering;
using CrazyShooter.Tools;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using CrazyShooter.Scene;

namespace CrazyShooter;

class Program
{
    private static IWindow graphicWindow;
    private static GL gl;
    private static uint program;
    private static Scene.Scene? currentScene;
    static void Main()
    {
        WindowOptions windowOptions = WindowOptions.Default;
        windowOptions.Title = "Crazy Shooter";
        windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

        graphicWindow = Window.Create(windowOptions);

        graphicWindow.Load += GraphicWindow_Load;
        graphicWindow.Update += GraphicWindow_Update;
        graphicWindow.Render += GraphicWindow_Render;
        graphicWindow.Closing += GraphicWindow_Closing;
        graphicWindow.FramebufferResize += size =>
        {
            gl.Viewport(size);
            if (currentScene is not null)
            {
                currentScene.AspectRatio = (float)size.X / size.Y;
            }
        };
        
        graphicWindow.Run();
    }
    
    private static void GraphicWindow_Load()
    {
        gl = graphicWindow.CreateOpenGL();
        program = ProgramUtils.LinkProgram(gl);
        gl.Enable(GLEnum.DepthTest);
        gl.DepthFunc(DepthFunction.Lequal);
        gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
    }
    
    private static void GraphicWindow_Closing()
    {
        if (currentScene is not null)
        {
            currentScene.Dispose();
        }
    }
    
    private static void GraphicWindow_Update(double deltaTime)
    {
        if (currentScene is not null)
        {
            currentScene.Update(deltaTime);
        }
    }

    private static unsafe void GraphicWindow_Render(double deltaTime)
    {
        if (currentScene is not null)
        {
            currentScene.Render(gl);
        }
    }
}