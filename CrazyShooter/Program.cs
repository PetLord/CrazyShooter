using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace CrazyShooter;

class Program
{
    private static IWindow graphicWindow;
    private static GL Gl;
    
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
            Gl.Viewport(size);
        };

        
        graphicWindow.Run();
    }
    
    
    private static void GraphicWindow_Load()
    {
        Gl = graphicWindow.CreateOpenGL();
        Gl.Enable(GLEnum.DepthTest);
        Gl.DepthFunc(DepthFunction.Lequal);
        Gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
    }
    
    private static void GraphicWindow_Closing()
    {

    }
    
    private static void GraphicWindow_Update(double deltaTime)
    {

    }

    private static unsafe void GraphicWindow_Render(double deltaTime)
    {
        
    }
}