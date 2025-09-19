using Godot;

public partial class OreNugget : RigidBody2D
{
    public int OreDensity;
    public Sprite2D Sprite;
    public MineableBlock.OreTypes OreType;
    private MetaprogressionManager metaprogressionManager;

    public override void _Ready()
    {
        metaprogressionManager = GetNode<MetaprogressionManager>("/root/MetaprogressionManager");
        Sprite = GetNode<Sprite2D>("Sprite");
        Sprite.Texture = GD.Load<Texture2D>("res://assets/sprites/oreNuggets/" + OreType + "Nugget.png");
        BodyEntered += Collision;
        var rng = new RandomNumberGenerator();
        ApplyTorqueImpulse(rng.RandiRange(-400, 400));
        ApplyImpulse(new Vector2(rng.RandfRange(-50, 50), 0));

    }

    private void Collision(Node body)
    {
        if (body is Player)
        {
            metaprogressionManager.AddOre(OreType, OreDensity);
            QueueFree();
        }
    }

}
