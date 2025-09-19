using System;
using Godot;

public partial class MineableBlock : StaticBody2D
{
    public Label CoordsLabel;
    private bool _isInTilemap;
    public HealthComponent healthComponent;
    public enum OreTypes { Gold, Iron, Carbon, Silver, Cobalt }
    [Export] public bool ContainsOre;
    [Export] public OreTypes OreType;
    [Export] public int OreAmount;
    public Sprite2D DestructionStage1;
    public Sprite2D DestructionStage2;
    public Sprite2D OreOverlay;
    private PackedScene _oreNuggetScene;

    public override void _EnterTree()
    {
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.OnDamage += UpdateDestructionStage;
        healthComponent.OnDeath += DropOre;
    }

    public override void _ExitTree()
    {
        healthComponent.OnDamage -= UpdateDestructionStage;
        healthComponent.OnDeath -= DropOre;
    }

    public void Init()
    {
        CoordsLabel = GetNode<Label>("CoordsLabel");
        AssignOreScene();
        DestructionStage1 = GetNode<Sprite2D>("DestructionStage1");
        DestructionStage2 = GetNode<Sprite2D>("DestructionStage2");
        if (!ContainsOre) return;
        OreOverlay = GetNode<Sprite2D>("OreOverlay");
        OreOverlay.Texture = GD.Load<Texture2D>("res://assets/sprites/oreOverlays/" + OreType + "OreOverlay.png");
        OreOverlay.Visible = ContainsOre;
    }

    public override void _Ready()
    {
        Init();
    }

    private void UpdateDestructionStage()
    {
        DestructionStage1.Visible = healthComponent.Health <= healthComponent.MaxHealth / 2;
        DestructionStage2.Visible = healthComponent.Health <= healthComponent.MaxHealth / 4;
    }

    private void DropOre()
    {
        if (ContainsOre)
        {
            var nugget = _oreNuggetScene.Instantiate<OreNugget>();
            nugget.OreType = OreType;
            nugget.OreDensity = OreAmount;
            switch (_isInTilemap)
            {
                case false:
                    nugget.GlobalPosition = GlobalPosition;
                    //GD.Print("block not in tilemap");
                    break;
                case true:
                    nugget.GlobalPosition = Position;
                    //GD.Print("block in tilemap");
                    break;
            }
            GetTree().GetRoot().AddChild(nugget);
        }
    }

    private void AssignOreScene()
    {
        if (!ContainsOre) return;
        _oreNuggetScene = GD.Load<PackedScene>("res://scenes/OreNugget.tscn");
    }


}
