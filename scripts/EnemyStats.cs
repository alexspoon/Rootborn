using Godot;

[GlobalClass]
public partial class EnemyStats : Resource
{
    [ExportSubgroup("Health")]
    [Export] public float MaxHealth;
    [Export] public bool Regenerative;
    [Export] public float RegenAmount;
    [Export] public float RegenCooldown;
    [Export] public float Toughness;

    [ExportSubgroup("Attack")]
    [Export] public float AttackDamage;
    [Export] public float AttackCooldown;
    [Export] public bool HasRangedAttack;
    [Export] public float RangedAttackDamage;
    [Export] public float RangedAttackCooldown;

    [ExportSubgroup("Movement")]
    [Export] public float TerminalVelocity;
    [Export] public float LocalGravity;
    [Export] public float MaxSpeed;
    [Export] public float GroundAcceleration;
    [Export] public float AirAcceleration;
    [Export] public float GroundDeceleration;
    [Export] public float AirDeceleration;
}
