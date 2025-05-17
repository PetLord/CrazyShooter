using CrazyShooter.Collision;
using CrazyShooter.Input;
using CrazyShooter.Rendering;
using CrazyShooter.Rendering.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = CrazyShooter.Rendering.Shader;

namespace CrazyShooter.Scene;

public class Scene : IDisposable
{
    private Camera Camera { get; } = new();
    private Player Player { get; set; }
    private List<GameObject> gameObjects = new List<GameObject>();
    public float AspectRatio { get; set; } = 16 / 9f;
    
    private Shader shader;
    private Shader skyBoxShader;
    private SkyBox skyBox;
    private DirectionalLight directionalLight;
    private List<PointLight> pointLights = new();
    private List<SpotLight> spotLights = new();

    private PlayerInputHandler playerInputHandler;
    
    public Scene(GL gl, Shader shader, Shader skyBoxShader, Player player, PlayerInputHandler playerInputHandler)
    {
        this.shader = shader;
        this.skyBoxShader = skyBoxShader;
        Player = player;
        directionalLight = new DirectionalLight();
        this.playerInputHandler = playerInputHandler;
        AddDefaultLights();
        skyBox = new SkyBox(gl, this.skyBoxShader);
        AddGameObject(GameObjectFactory.CreateFloor(gl, shader));
    }

    public Scene(GL gl, Shader shader, Shader skyBoxShader, PlayerInputHandler playerInputHandler)
    {
        this.shader = shader;
        this.skyBoxShader = skyBoxShader;
        Player = GameObjectFactory.CreatePlayer(gl, shader);
        directionalLight = new DirectionalLight();
        this.playerInputHandler = playerInputHandler;
        AddDefaultLights();
        skyBox = new SkyBox(gl, this.skyBoxShader);
        AddGameObject(GameObjectFactory.CreateFloor(gl, shader));
        AddRandomObjects(gl, shader, 5);
    }

    private void AddRandomObjects(GL gl, Shader shader, int number)
    {
        //AddGameObject(GameObjectFactory.CreateCollidableGameObject(gl, shader, Assets.Models.Cactus, Assets.Textures.Cactus));
        var palm1 = GameObjectFactory.LoadPalm1(gl, shader);
        palm1.Position = new Vector3D<float>(5f, 0f, 5f);
        AddGameObject(palm1);
    }
    
    public bool TryMove(CollidableObject movingObject, Vector3D<float> movement)
    {
        // Check for current penetration
        var currentBounds = movingObject.BoundingBox;
        foreach (var obj in gameObjects)
        {
            if (obj == movingObject) continue;
            if (obj is ICollidable collidable && currentBounds.Intersects(collidable.BoundingBox))
            {
                // They are already intersecting → resolve
                ResolvePenetration(movingObject, collidable);
                return false;
            }

            if (obj is CompositeGameObject compositeGameObject)
            {
                foreach (var child in compositeGameObject.GetChildren())
                {
                    if (child is ICollidable collidableChild && currentBounds.Intersects(collidableChild.BoundingBox))
                    {
                        ResolvePenetration(movingObject, collidableChild);
                        return false;
                    }
                }
            }
        }
        
        // Compute proposed bounding box
        var proposedPosition = movingObject.Position + movement;
        BoundingBox proposedBounds = new BoundingBox(
            proposedPosition + movingObject.MeshMinBounds * movingObject.Scale,
            proposedPosition + movingObject.MeshMaxBounds * movingObject.Scale
        );
        
        foreach (var obj in gameObjects)
        {
            if (obj == movingObject) continue;
            if (obj is ICollidable collidable && proposedBounds.Intersects(collidable.BoundingBox))
            {
                return false;
            }
            
            if (obj is CompositeGameObject compositeGameObject)
            {
                foreach (var child in compositeGameObject.GetChildren())
                {
                    if (child is ICollidable collidableChild && proposedBounds.Intersects(collidableChild.BoundingBox))
                    {
                        return false;
                    }
                }
            }
        }

        movingObject.Position = proposedPosition;
        return true;
    }
    
    void ResolvePenetration(CollidableObject obj, ICollidable other)
    {
        var a = obj.BoundingBox;
        var b = other.BoundingBox;

        var overlapX = MathF.Min(a.Max.X, b.Max.X) - MathF.Max(a.Min.X, b.Min.X);
        var overlapY = MathF.Min(a.Max.Y, b.Max.Y) - MathF.Max(a.Min.Y, b.Min.Y);
        var overlapZ = MathF.Min(a.Max.Z, b.Max.Z) - MathF.Max(a.Min.Z, b.Min.Z);

        // Find the smallest axis to push on
        if (overlapX < overlapY && overlapX < overlapZ)
        {
            float pushX = (a.Center.X < b.Center.X) ? -overlapX : overlapX;
            obj.Position = new Vector3D<float>(obj.Position.X + pushX, obj.Position.Y, obj.Position.Z);
        }
        else if (overlapY < overlapZ)
        {
            float pushY = (a.Center.Y < b.Center.Y) ? -overlapY : overlapY;
            obj.Position = new Vector3D<float>(obj.Position.X, obj.Position.Y + pushY, obj.Position.Z);
        }
        else
        {
            float pushZ = (a.Center.Z < b.Center.Z) ? -overlapZ : overlapZ;
            obj.Position = new Vector3D<float>(obj.Position.X, obj.Position.Y, obj.Position.Z + pushZ);
        }
    }

    
    public void Update(double deltaTime)
    {
        playerInputHandler.ProcessInput(Player, deltaTime, Camera, this);
        Camera.Follow(Player.Position, Player.Rotation.Y, Player.Rotation.X);
        Player.Update(deltaTime);
        foreach (var t in gameObjects)
        {
            t.Update(deltaTime);
        }
    }
    
    public void Render(GL gl)
    {
        gl.Clear((uint)(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit));
        gl.DepthFunc(GLEnum.Lequal);
        
        var viewMatrix = Camera.GetViewMatrix();
        var projectionMatrix = Camera.GetProjectionMatrix(AspectRatio);
        
        shader.Use();
        SetLights(gl);
        
        Player.Render(viewMatrix, projectionMatrix);
        foreach (var t in gameObjects)
        {
            t.Render(viewMatrix, projectionMatrix);
        }
        
        gl.DepthMask(false);
        
        skyBoxShader.Use();
        skyBox.RenderSkybox(viewMatrix, projectionMatrix);
        
        gl.DepthMask(true);
    }

    private void SetLights(GL gl)
    {
        int dirDirLoc = gl.GetUniformLocation(shader.Handle, "dirLight.direction");
        gl.Uniform3(dirDirLoc, directionalLight.Direction.X, directionalLight.Direction.Y, directionalLight.Direction.Z);

        int dirAmbientLoc = gl.GetUniformLocation(shader.Handle, "dirLight.ambient");
        gl.Uniform3(dirAmbientLoc, directionalLight.Ambient.X, directionalLight.Ambient.Y, directionalLight.Ambient.Z);

        int dirDiffuseLoc = gl.GetUniformLocation(shader.Handle, "dirLight.diffuse");
        gl.Uniform3(dirDiffuseLoc, directionalLight.Diffuse.X, directionalLight.Diffuse.Y, directionalLight.Diffuse.Z);

        int dirSpecLoc = gl.GetUniformLocation(shader.Handle, "dirLight.specular");
        gl.Uniform3(dirSpecLoc, directionalLight.Specular.X, directionalLight.Specular.Y, directionalLight.Specular.Z);

        // Set number of point lights (limited by MAX_POINT_LIGHTS in shader)
        int numPointLoc = gl.GetUniformLocation(shader.Handle, "numPointLights");
        gl.Uniform1(numPointLoc, Math.Min(pointLights.Count, 4));

        // Loop through point lights and set their uniforms
        for (int i = 0; i < pointLights.Count && i < 4; i++)
        {
            var pl = pointLights[i];
            string prefix = $"pointLights[{i}]";
            gl.Uniform3(gl.GetUniformLocation(shader.Handle, prefix + ".position"), pl.Position.X, pl.Position.Y, pl.Position.Z);
            gl.Uniform3(gl.GetUniformLocation(shader.Handle, prefix + ".ambient"), pl.Ambient.X, pl.Ambient.Y, pl.Ambient.Z);
            gl.Uniform3(gl.GetUniformLocation(shader.Handle, prefix + ".diffuse"), pl.Diffuse.X, pl.Diffuse.Y, pl.Diffuse.Z);
            gl.Uniform3(gl.GetUniformLocation(shader.Handle, prefix + ".specular"), pl.Specular.X, pl.Specular.Y, pl.Specular.Z);

            gl.Uniform1(gl.GetUniformLocation(shader.Handle, prefix + ".constant"), pl.Constant);
            gl.Uniform1(gl.GetUniformLocation(shader.Handle, prefix + ".linear"), pl.Linear);
            gl.Uniform1(gl.GetUniformLocation(shader.Handle, prefix + ".quadratic"), pl.Quadratic);
        }

        // Also set the camera position uniform for specular lighting
        var viewPosLoc = gl.GetUniformLocation(shader.Handle, "viewPos");
        var camPos = Camera.Position;
        gl.Uniform3(viewPosLoc, camPos.X, camPos.Y, camPos.Z);

    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
    }

    public void AddPointLight(PointLight light)
    {
        pointLights.Add(light);
    }

    public void AddSpotLight(SpotLight light)
    {
        spotLights.Add(light);
    }
    
    public void Dispose()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.Dispose();
        }
        Player.Dispose();
        
    }

    private void AddDefaultLights()
    {
        PointLight pointLight = new PointLight();
        pointLight.Position = new Vector3D<float>(10F, 10F, 10F);
        AddPointLight(pointLight);
        
        this.directionalLight = new DirectionalLight();
    }
}