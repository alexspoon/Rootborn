using Godot;
[GlobalClass]

public partial class Enemy : CharacterBody2D
{
    [Export] public EnemyStats enemyStats;
    public bool ChasingPlayer;
    public HealthComponent healthComponent;
    public Vector2 targetVelocity;
    public Player player;

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
        healthComponent.MaxHealth = enemyStats.MaxHealth;
        healthComponent.Toughness = enemyStats.Toughness;
        healthComponent.FullHeal();
        healthComponent.OnDamage += ApplyKnockback;
    }

    public void ApplyKnockback()
    {

    }

    public virtual void ChasePlayer()
    {
        if (player == null) return;
        Vector2 movementDirection = (player.GlobalPosition - GlobalPosition).Normalized();
        var velocityChange = 0f;
        if (IsOnFloor())
        {
            switch (ChasingPlayer)
            {
                case true:
                    velocityChange = enemyStats.GroundAcceleration;
                    break;
                case false:
                    velocityChange = enemyStats.GroundDeceleration;
                    break;
            }
        }
        else
        {
            ApplyGravity();
            switch (ChasingPlayer)
            {
                case true:
                    velocityChange = enemyStats.AirAcceleration;
                    break;
                case false:
                    velocityChange = enemyStats.AirDeceleration;
                    break;
            }
        }
        if (!ChasingPlayer) movementDirection = Vector2.Zero;
        if (IsOnFloor()) targetVelocity.Y = 0;
        targetVelocity.X = Mathf.MoveToward(targetVelocity.X, movementDirection.X * enemyStats.MaxSpeed, velocityChange);
        Velocity = targetVelocity;
        MoveAndSlide();
    }
    public virtual void ApplyGravity()
    {
        targetVelocity.Y += enemyStats.LocalGravity;
        targetVelocity = new Vector2(targetVelocity.X, Mathf.Min(targetVelocity.Y, enemyStats.TerminalVelocity));
    }

    
}
