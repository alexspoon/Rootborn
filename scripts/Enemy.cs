using Godot;
[GlobalClass]

public partial class Enemy : CharacterBody2D
{
    [Export] public EnemyStats enemyStats;
    public HealthComponent healthComponent;
    public Player player;
    public bool Regenerative;
    public float RegenAmount;
    public float RegenCooldown;
    public float Speed;
    public float AttackDamage;
    public float AttackCooldown;
    public float RangedAttackDamage;
    public float RangedAttackCooldown;

    public override void _Ready()
    {
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        Init();
        FindPlayer();
    }

    public void FindPlayer()
    {
        player = GetTree().GetRoot().GetNode<Node>("Main").GetChild<Level>(0).GetNodeOrNull<Player>("Player");
    }

    public virtual void Init()
    {
        Regenerative = enemyStats.Regenerative;
        RegenAmount = enemyStats.RegenAmount;
        RegenCooldown = enemyStats.RegenCooldown;
        healthComponent.MaxHealth = enemyStats.MaxHealth;
        healthComponent.Toughness = enemyStats.Toughness;
        Speed = enemyStats.Speed;
        AttackDamage = enemyStats.AttackDamage;
        AttackCooldown = enemyStats.AttackCooldown;
        RangedAttackDamage = enemyStats.RangedAttackDamage;
        RangedAttackCooldown = enemyStats.RangedAttackCooldown;
        healthComponent.FullHeal();
    }
    public virtual void ChasePlayer()
    {
        
    }
}
