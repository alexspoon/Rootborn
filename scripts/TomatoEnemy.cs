using Godot;
using System;

public partial class TomatoEnemy : Enemy
{
    private float jumpStrength = 400;
    private bool canJump = true;
    private Area2D aggroRadius;
    private Area2D hurtBox;
    private Timer jumpCooldown;
    private Timer aggroCooldown;
    private Timer positionCheck;
    private Vector2 movementDirection;
    public override void _Ready()
    {
        base._Ready();
        hurtBox = GetNode<Area2D>("HurtBox");
        aggroRadius = GetNode<Area2D>("AggroRadius");
        jumpCooldown = GetNode<Timer>("JumpCooldown");
        aggroCooldown = GetNode<Timer>("AggroCooldown");
        positionCheck = GetNode<Timer>("PositionCheck");
        aggroRadius.BodyEntered += OnAggroEnter;
        aggroRadius.BodyExited += OnAggroExit;
        jumpCooldown.Timeout += OnJumpCooldown;
        aggroCooldown.Timeout += OnAggroCooldown;
        positionCheck.Timeout += PositionCheck;
        hurtBox.BodyEntered += OnCollision;
    }

    private void Jump()
    {
        if (IsQueuedForDeletion()) return;
        if (!canJump) return;
        targetVelocity.Y -= jumpStrength;
        canJump = false;
        jumpCooldown.Start();
        GD.Print(Name + " jumped!");
    }

    private void OnJumpCooldown()
    {
        if (IsQueuedForDeletion()) return;
        if (!IsInstanceValid(jumpCooldown)) return;
        canJump = true;
    }

    private void OnAggroCooldown()
    {
        if (IsQueuedForDeletion()) return;
        if (!IsInstanceValid(aggroCooldown)) return;
        ChasingPlayer = false;
    }

    private void OnAggroEnter(Node body)
    {
        if (IsQueuedForDeletion()) return;
        if (!IsInstanceValid(player)) return;
        if (body != player) return;
        ChasingPlayer = true;
    }

    private void OnAggroExit(Node body)
    {
        if (IsQueuedForDeletion()) return;
        if (!IsInstanceValid(player)) return;
        if (body != player) return;
        aggroCooldown.Start();
    }

    private void OnCollision(Node body)
    {
        if (body != player) return;
        if (body.GetNodeOrNull<HealthComponent>("HealthComponent") == null) return;
        var healthComponent = body.GetNode<HealthComponent>("HealthComponent");
        if (healthComponent.Invulnerable) return;
        healthComponent.TakeDamage(enemyStats.AttackDamage);
    }

    private void PositionCheck()
    {
        if (IsQueuedForDeletion()) return;
        if (!IsInstanceValid(player)) return;
        movementDirection = (player.GlobalPosition - GlobalPosition).Normalized();
    }

    public override void ChasePlayer()
    {
        if (IsQueuedForDeletion()) return;
        if (player == null) return;
        var velocityChange = 0f;
        if (IsOnFloor())
        {
            enemyStats.MaxSpeed = 140;
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
            enemyStats.MaxSpeed = 200;
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
        if (!IsInstanceValid(player)) return;
        if ((player.GlobalPosition - GlobalPosition).Length() < 128 && (player.GlobalPosition - GlobalPosition).Length() > 64)
        {
            Jump();
        }
        targetVelocity.X = Mathf.MoveToward(targetVelocity.X, movementDirection.X * enemyStats.MaxSpeed, velocityChange);
        Velocity = targetVelocity;
        MoveAndSlide();
    }


    public override void _PhysicsProcess(double delta)
    {
        if (IsQueuedForDeletion()) return;
        ChasePlayer();
    }


}
