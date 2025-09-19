using Godot;

[GlobalClass]
public partial class EnemyStats : Resource
{
    [Export] public float MaxHealth;
    [Export] public bool Regenerative;
    [Export] public float RegenAmount;
    [Export] public float RegenCooldown;
    [Export] public float Toughness;
    [Export] public float AttackDamage;
    [Export] public float AttackCooldown;
    [Export] public float Speed;
    [Export] public bool HasRangedAttack;
    [Export] public float RangedAttackDamage;
    [Export] public float RangedAttackCooldown;
    [Export] public float PhysicsMass;
    [Export] public float PhysicsGravityScale;
}
