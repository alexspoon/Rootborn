using Godot;

public partial class Player : CharacterBody2D
{
    [Export] public PlayerStats playerStats;
    [Export] public PackedScene _sceneToSpawn;
    public Sprite2D Sprite;
    private SaveManager _saveManager;
    private LevelManager _levelManager;
    public HealthComponent healthComponent;
    public Area2D PickupArea;
    public PlayerCamera playerCamera;
    private GpuParticles2D _trail;
    private ProgressBar healthBar;

    public override void _Ready()
    {
        playerCamera = GetParent().GetNodeOrNull<PlayerCamera>("PlayerCamera");
        _saveManager = GetNode<SaveManager>("/root/SaveManager");
        _levelManager = GetNode<LevelManager>("/root/LevelManager");
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        Sprite = GetNode<Sprite2D>("Sprite");
        PickupArea = GetNode<Area2D>("PickupArea");
        healthBar = GetNode<ProgressBar>("HealthBar");
        _trail = GetNode<GpuParticles2D>("Trail");

        UpdateStats();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("inputEscapeMenu")) _levelManager.SwitchLevel("MainMenu", false);
        //if (Input.IsActionJustPressed("inputEquipTool")) _saveManager.SaveMetaprogression("test.json");

        _trail.GlobalPosition = GlobalPosition;
        healthBar.Value = healthComponent.Health;
        healthBar.MaxValue = healthComponent.MaxHealth;

        Pickup();
        DebugTools();
    }
    public void UpdateStats()
    {
        healthComponent.MaxHealth = playerStats.MaxHealth * (playerStats.BonusPercentageMaxHealth + 1) + playerStats.BonusFlatMaxHealth;
        healthComponent.Toughness = playerStats.Toughness * (playerStats.BonusPercentageToughness + 1) + playerStats.BonusFlatToughness;
        healthComponent.FullHeal();
    }

    public void Pickup()
    {
        if (PickupArea.GetOverlappingBodies() == null) return;
        foreach (var body in PickupArea.GetOverlappingBodies())
        {
            if (body is OreNugget)
            {
                var nugget = (OreNugget)body;
                nugget.ApplyCentralForce((GlobalPosition - nugget.GlobalPosition) * 15);
            }
        }
    }

    private void DebugTools()
    {
        DebugSpawnSceneAtCursor(_sceneToSpawn);
        DebugTeleportPlayerToCursor();
    }

    private void DebugTeleportPlayerToCursor()
    {
        if (playerCamera == null) return;
        if (!Input.IsActionJustPressed("inputZoomReset")) return;
        var targetPos = playerCamera.GetGlobalMousePosition();
        GlobalPosition = targetPos;
    }

    private bool DebugSpawnSceneAtCursor(PackedScene sceneToSpawn)
    {
        if (playerCamera == null) return false;
        if (!Input.IsActionJustPressed("inputThrow")) return false;
        var targetPos = playerCamera.GetGlobalMousePosition();
        var spawnedScene = sceneToSpawn.Instantiate<Node2D>();
        spawnedScene.GlobalPosition = targetPos;
        GetParent().AddChild(spawnedScene);
        GD.Print("spawned enemy at " + targetPos);
        return true;
    }
}