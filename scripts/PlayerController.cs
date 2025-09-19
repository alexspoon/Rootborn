using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerController : Node
{
    private Player player;
    #region MovementStats
    [ExportSubgroup("Movement Properties")]
    [Export] public bool SnapInput = true;
    [Export] public float MaxSpeed = 220f;
    [Export] public float Acceleration = 50f;
    [Export] public float GroundDeceleration = 20f;
    [Export] public float AirDeceleration = 5f;
    [Export] public float JumpStrength = -520f;
    [Export] public float FallAceleration = 40f;
    [Export] public float TerminalVelocity = 400f;
    [Export] public float JumpEndEarlyGravityModifier = 3f;
    [Export] public float CoyoteTimeDuration = .2f;
    [Export] public float JumpBufferTime = .2f;
    [Export] public int MaxJumps = 1;
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 0.5f;
    #endregion

    public override void _Ready()
    {
        player = GetParent<Player>();
        attackArea = player.GetNode<Area2D>("AttackArea");
        animationPlayer = player.GetNode<AnimationPlayer>("AnimationPlayer");
        gravityTimer = new();
        gravityTimer.WaitTime = 0.15f;
        gravityTimer.OneShot = true;
        gravityTimer.Timeout += ResetGravity;
        AddChild(gravityTimer);
        dashTimer = new();
        dashTimer.WaitTime = DashDuration;
        dashTimer.OneShot = true;
        dashTimer.Timeout += DashEnd;
        AddChild(dashTimer);
        dashCooldownTimer = new();
        dashCooldownTimer.WaitTime = DashCooldown;
        dashCooldownTimer.OneShot = true;
        dashCooldownTimer.Timeout += OnDashCooldown;
        AddChild(dashCooldownTimer);
        wallJumpTimer = new();
        wallJumpTimer.WaitTime = 0.1f;
        wallJumpTimer.OneShot = true;
        wallJumpTimer.Timeout += WallJumpTimerTimeout;
        AddChild(wallJumpTimer);
    }

    public override void _PhysicsProcess(double delta)
    {
        grounded = player.IsOnFloor();
        Movement(delta);
        Attack();
        previousYVelocity = targetVelocity.Y;
    }

    #region Attacking
    private bool isAttacking = false;
    private bool canAttack = true;
    private enum attackDirections
    {
        Up,
        Left,
        Right,
        Down
    }
    private attackDirections attackDirection;
    private Area2D attackArea;
    private AnimationPlayer animationPlayer;
    private void CheckAttackDirection()
    {
        if (Input.IsActionPressed("inputUp"))
        {
            if (!canAttack) return;
            attackArea.Rotation = Mathf.DegToRad(-90f);
            attackArea.Position = new Vector2(0, -6);
            attackDirection = attackDirections.Up;
        }
        if (Input.IsActionPressed("inputLeft"))
        {
            if (!isOnWall) player.RotationDegrees = Mathf.Lerp(player.RotationDegrees, -5, 0.1f);
            if (!canAttack) return;
            attackArea.Rotation = Mathf.DegToRad(180f);
            attackArea.Position = Vector2.Zero;
            attackDirection = attackDirections.Left;
        }
        if (Input.IsActionPressed("inputRight"))
        {
            if (!isOnWall) player.RotationDegrees = Mathf.Lerp(player.RotationDegrees, 5, 0.1f);
            if (!canAttack) return;
            attackArea.Rotation = 0;
            attackArea.Position = Vector2.Zero;
            attackDirection = attackDirections.Right;
        }
        if (Input.IsActionPressed("inputDown"))
        {
            if (!canAttack) return;
            attackArea.Rotation = Mathf.DegToRad(90f);
            attackArea.Position = new Vector2(0, 6);
            attackDirection = attackDirections.Down;
        }
    }
    private void Attack()
    {
        CheckAttackDirection();
        if (isAttacking) HitCheck();
        if (Input.IsActionJustPressed("inputPrimaryAttack") && canAttack)
        {
            switch (attackDirection)
            {
                case attackDirections.Up:
                    animationPlayer.Play("AttackUp");
                    break;
                case attackDirections.Left:
                    animationPlayer.Play("AttackLeft");
                    break;
                case attackDirections.Right:
                    animationPlayer.Play("AttackRight");
                    break;
                case attackDirections.Down:
                    animationPlayer.Play("AttackDown");
                    break;
            }
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        isAttacking = true;
    }

    private void StopHitCheck()
    {
        isAttacking = false;
    }

    private void FinishAttack()
    {
        canAttack = true;
    }

    private void HitCheck()
    {
        GD.Print(time + " hitcheck");
        foreach (var body in attackArea.GetOverlappingBodies())
        {
            if (body.GetNodeOrNull<HealthComponent>("HealthComponent") == null) return;
            var healthComponent = body.GetNode<HealthComponent>("HealthComponent");
            if (healthComponent.Invulnerable) return;
            if (attackDirection == attackDirections.Down)
            {
                localGravity = 20;
                gravityTimer.Start();
                targetVelocity.Y = JumpStrength;
            }
            healthComponent.TakeDamage(10);
        }
    }

    #endregion
    #region Movement
    private Vector2 targetVelocity = Vector2.Zero;
    private float previousYVelocity;
    private float localGravity = 40;
    private Timer gravityTimer;
    private Timer dashTimer;
    private Timer dashCooldownTimer;
    private Timer wallJumpTimer;
    private float time;
    private bool grounded;
    private bool canJump = true;
    private bool jumpBuffered;
    private bool coyoteJump;
    private int jumpCount;
    private bool canDash = true;
    private bool isDashing = false;
    private bool isOnWall;
    private bool canWallDrag = true;
    private float horizontalInput;
    private void Movement(double delta)
    {
        isOnWall = player.IsOnWallOnly();
        horizontalInput = Input.GetAxis("inputLeft", "inputRight");
        if (player.IsOnCeiling() && targetVelocity.Y < 0) targetVelocity.Y = Mathf.Lerp(targetVelocity.Y, 0, 0.1f);
        if (isOnWall && targetVelocity.X != 0) targetVelocity.X = Mathf.Lerp(targetVelocity.X, 0, 0.1f);
        if (!canWallDrag) horizontalInput = 0;
        if (horizontalInput == 0)
        {
            player.Rotation = Mathf.Lerp(player.Rotation, 0, 0.1f);
        }
        if (horizontalInput > 0)
        {
            player.Sprite.FlipH = false;
        }
        else if (horizontalInput < 0)
        {
            player.Sprite.FlipH = true;
        }
        var velocityChange = 0f;
        if (player.IsOnFloor())
        {
            if (horizontalInput == 0)
            {
                velocityChange = GroundDeceleration;
                MaxSpeed = 220;
            }
            else
            {
                velocityChange = Acceleration;
                MaxSpeed = 220;
            }
        }
        else
        {
            if (horizontalInput == 0)
            {
                velocityChange = AirDeceleration;
            }
            else
            {
                velocityChange = Acceleration + 25;
                MaxSpeed = 250;
            }
        }
        targetVelocity.X = Mathf.MoveToward(targetVelocity.X, horizontalInput * MaxSpeed, velocityChange);
        if (Input.IsActionJustPressed("inputDash")) DashStart();
        if (player.IsOnFloor())
        {
            jumpCount = 0;
            targetVelocity.Y = 0;
            coyoteJump = false;
            if (!gravityTimer.IsStopped()) gravityTimer.Stop();
            if (localGravity != 40f) ResetGravity();
        }
        if (!player.IsOnFloor()) ApplyGravity();
        if (Input.IsActionJustPressed("inputJump"))
        {
            if (isOnWall)
            {
                ResetGravity();
                WallJump();
                return;
            }
            if (jumpCount >= MaxJumps) canJump = false;
            if (grounded) canJump = true;
            else if (!coyoteJump)
            {
                CoyoteTime();
            }
            switch (canJump)
            {
                case true:
                    Jump();
                    break;
                case false:
                    JumpBuffer();
                    break;
            }
        }
        if (grounded && jumpBuffered && !isDashing) Jump();
        player.Velocity = targetVelocity;
        player.MoveAndSlide();
    }
    private async void JumpBuffer()
    {
        jumpBuffered = true;
        var timer = GetTree().CreateTimer(JumpBufferTime);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        jumpBuffered = false;
    }
    private async void CoyoteTime()
    {
        if (jumpCount >= MaxJumps) return;
        coyoteJump = true;
        canJump = true;
        var timer = GetTree().CreateTimer(CoyoteTimeDuration);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        canJump = false;
    }
    private void Jump()
    {
        if (isDashing || isOnWall) ResetGravity();
        targetVelocity.Y += JumpStrength;
        jumpCount++;
    }
    private void WallJump()
    {
        ResetGravity();
        wallJumpTimer.Start();
        canWallDrag = false;
        targetVelocity.Y += JumpStrength -30;
        targetVelocity.X += CheckWallDirection() * 500;
        jumpCount = 0;
        jumpCount++;
    }
    private void DashStart()
    {
        if (!canDash) return;
        isDashing = true;
        canDash = false;
        var localHorizontalInput = Mathf.RoundToInt(horizontalInput);
        if (localHorizontalInput == 0)
        {
            DashEnd();
            dashCooldownTimer.Start();
            return;
        }
        player.RotationDegrees = localHorizontalInput * 45f;
        targetVelocity.X = localHorizontalInput * 900;
        targetVelocity.Y = 0;
        localGravity = 0;
        dashTimer.Start();
        dashCooldownTimer.Start();
    }
    private void OnDashCooldown()
    {
        canDash = true;
    }
    private void DashEnd()
    {
        isDashing = false;
        ResetGravity();
    }
    private void WallJumpTimerTimeout()
    {
        canWallDrag = true;
    }
    private void ApplyGravity()
    {
        if (isOnWall && canWallDrag)
        {
            var localHorizontalInput = Mathf.RoundToInt(horizontalInput);
            if (localHorizontalInput != CheckWallDirection()) localGravity = Mathf.Lerp(0, 40f, 0.05f);
            if (targetVelocity.Y != 0) targetVelocity.Y = Mathf.Lerp(targetVelocity.Y, 0, 0.1f);
        }
        else if (localGravity != 40 && gravityTimer.IsStopped()) ResetGravity();
        targetVelocity.Y += localGravity;
        if (previousYVelocity < 0.01 && targetVelocity.Y > 0)
        {
            if (!gravityTimer.IsStopped()) return;
            if (!canDash) return;
            if (isOnWall) return;
            GD.Print("apex");
            localGravity = 20f;
            gravityTimer.Start();
        }
        if (Input.IsActionJustReleased("inputJump") && !player.IsOnFloor())
        {
            if (targetVelocity.Y < 0) targetVelocity.Y = -targetVelocity.Y / 2;
        }
        targetVelocity = new Vector2(targetVelocity.X, Mathf.Min(targetVelocity.Y, TerminalVelocity));
    }
    private void ResetGravity()
    {
        localGravity = 40f;
    }

    private int CheckWallDirection()
    {
        for (int i = 0; i < player.GetSlideCollisionCount(); i++)
        {
            var collision = player.GetSlideCollision(i);
            if (collision.GetNormal().X < 0) return -1;
            else if (collision.GetNormal().X > 0) return 1;
        } return 0;
    }
    #endregion

}
