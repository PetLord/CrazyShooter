using CrazyShooter.Input;
using CrazyShooter.Rendering;
using CrazyShooter.Tools;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using CrazyShooter.Scene;
using CrazyShooter.UI;
using Silk.NET.Input;

using Shader = CrazyShooter.Rendering.Shader;
namespace CrazyShooter;

// todo! fix font style

class Program
{
    private enum GameState
    {
        MainMenu,
        Playing,
        Paused
    }

    private static IWindow graphicWindow;
    private static IInputContext inputContext;
    
    private static GL gl;
    private static uint program;
    private static Shader shader;
    private static Shader skyBoxShader;

    private static GameState gameState = GameState.MainMenu;
    private static Scene.Scene? currentScene;
    private static Menu? menu;
    private static PlayerInputHandler playerInputHandler;
    
    private static string vertexSource = ShaderUtils.GetEmbeddedResourceAsString("Assets.Shaders.OmniVertexShader.vert");
    private static string fragmentSource = ShaderUtils.GetEmbeddedResourceAsString("Assets.Shaders.OmniFragmentShader.frag");
    private static string skyBoxVertexSource = ShaderUtils.GetEmbeddedResourceAsString("Assets.Shaders.SkyBoxVertexShader.vert");
    private static string skyBoxFragmentSource = ShaderUtils.GetEmbeddedResourceAsString("Assets.Shaders.SkyBoxFragmentShader.frag");
    
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
            menu.WindowResized(size);
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
        shader = new Shader(gl, vertexSource, fragmentSource);
        skyBoxShader = new Shader(gl, skyBoxVertexSource, skyBoxFragmentSource);
        program = shader.Handle;
        // program = ProgramUtils.LinkProgram(gl);
        inputContext = graphicWindow.CreateInput();
        menu = new Menu(gl, graphicWindow, inputContext);
        playerInputHandler = new PlayerInputHandler(inputContext.Keyboards.ToArray(), inputContext.Mice.ToArray());
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
        switch (gameState)
        {
            case GameState.Playing:
                currentScene?.Update(deltaTime);
                break;

            case GameState.MainMenu:
                menu?.Update(deltaTime);
                break;
            
            case GameState.Paused:
                break;
        }
    }

    private static unsafe void GraphicWindow_Render(double deltaTime)
    {
        switch (gameState)
        {
            case GameState.Playing:
                RenderPlaying();
                break;

            case GameState.MainMenu:
                RenderMenu();
                break;
            
            case GameState.Paused:
                RenderPaused();
                break;
        }
    }

    private static unsafe void RenderPlaying()
    {
        if (currentScene is not null)
        {
            currentScene.Render(gl);
        }
    }

    private static unsafe void RenderMenu()
    {
        if (menu is not null)
        {
            menu.Render();
        }
    }

    private static unsafe void RenderPaused()
    {
        // to do
    }

    public static void StartGame()
    {
        gameState = GameState.Playing;
        currentScene = new Scene.Scene(gl, shader, skyBoxShader, playerInputHandler);
        playerInputHandler.SetMouseMode(PlayerInputHandler.MouseState.PlayMode);
    }
    
    public static void ExitGame()
    {
        System.Environment.Exit(0);
    }
    
}