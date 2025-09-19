using Godot;
[GlobalClass]

public partial class Weapon : Node2D
{
    public Sprite2D weaponSprite;
    public virtual void PrimaryFire() { }
    public virtual void SecondaryFire() { }
    [Export] public WeaponStats weaponStats;
    public override void _Ready()
    {

    }
}
