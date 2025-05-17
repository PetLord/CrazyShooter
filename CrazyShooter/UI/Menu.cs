using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.IO;
using CrazyShooter.Scene;

namespace CrazyShooter.UI;

public class Menu : IDisposable
{
    private readonly GL _gl;
    private static ImGuiController controller;
    private static ImGuiIOPtr io;
    private static ImFontPtr fontPtr;

    public Menu(GL gl, IWindow window, IInputContext inputContext)
    {
        _gl = gl;
        controller = new ImGuiController(gl, window, inputContext);
        io = ImGui.GetIO();

        // Set the ImGui style once
        ImGui.StyleColorsDark();

        if (File.Exists(Assets.Fonts.Germania))
        {
            // fontPtr = io.Fonts.AddFontFromFileTTF(fontPath, 24, null, io.Fonts.GetGlyphRangesDefault());
        }
        else
        {
            Console.WriteLine("Font not found: " + Assets.Fonts.Germania);
        }
    }

    public void Update(double deltaTime)
    {
        controller.Update((float)deltaTime);
    }
    
    public void Render()
    {
        RenderLayout();
        controller.Render(); 
    }

    private void RenderLayout()
    {
        ImGui.SetNextWindowSize(new Vector2(400, 300));
        ImGui.SetNextWindowPos(new Vector2(
            ImGui.GetIO().DisplaySize.X / 2f - 200,
            ImGui.GetIO().DisplaySize.Y / 2f - 150
        ));
        ImGui.Begin("Main Menu", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar);
        
        // Assuming font has been pushed already, no need to push again
        ImGui.SetCursorPosY(30);
        ImGui.SetCursorPosX((400 - ImGui.CalcTextSize("Crazy Shooter").X) / 2f);
        ImGui.PushFont(fontPtr); 
        ImGui.Text("Crazy Shooter");
        ImGui.PopFont();

        ImGui.Dummy(new Vector2(0, 30)); // spacer

        // Button styling
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 6.0f);
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.4f, 0.7f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.5f, 0.8f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.1f, 0.3f, 0.6f, 1f));

        float buttonWidth = 200f;
        float buttonHeight = 40f;
        float centerX = (400 - buttonWidth) / 2f;

        ImGui.SetCursorPosX(centerX);
        if (ImGui.Button("Start Game", new Vector2(buttonWidth, buttonHeight)))
        {
            Program.StartGame();
        }

        ImGui.Dummy(new Vector2(0, 20)); // Spacing

        ImGui.SetCursorPosX(centerX);
        if (ImGui.Button("Exit", new Vector2(buttonWidth, buttonHeight)))
        {
            Program.ExitGame();
        }

        ImGui.PopStyleColor(3);  // Pop button style colors
        ImGui.PopStyleVar();     // Pop style var
        ImGui.End();
    }

    public void WindowResized(Vector2D<int> size)
    {
        io.DisplaySize = new Vector2(size.X, size.Y);
        ImGui.SetNextWindowSize(new Vector2(size.X, size.Y)); 
        ImGui.SetNextWindowPos(new Vector2(size.X / 2f - 200, size.Y / 2f - 150)); 
    }

    public void Dispose()
    {
        // Dispose of resources here, if needed
    }
}
