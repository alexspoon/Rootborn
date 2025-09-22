using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerController : Node
{
    private Player player;
    private MeshInstance2D mesh;
    public float VelocityChange;
    
    #region MovementStats
    [ExportSubgroup("Movement Properties")]
    [Export] public bool SnapInput = true;
    [Export] public float MaxSpeed = 220f;
    [Export] public float Acceleration = 50f;
    [Export] public float AirAcceleration = 75f;
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
        mesh = player.GetNode<MeshInstance2D>("DebugMesh");
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
        previousYVelocity = TargetVelocity.Y;
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
        if (isOnWall)
        {
            if (Mathf.RoundToInt(HorizontalInput) == 1)
            {
                if (!canAttack) return;
                attackArea.GlobalRotation = Mathf.DegToRad(180f);
                attackArea.Position = Vector2.Zero;
                attackArea.Position = new Vector2(-6, 0);
                attackDirection = attackDirections.Left;
            }
            if (Mathf.RoundToInt(HorizontalInput) == -1)
            {
                if (!canAttack) return;
                attackArea.GlobalRotation = 0;
                attackArea.Position = Vector2.Zero;
                attackArea.Position = new Vector2(6, 0);
                attackDirection = attackDirections.Right;
            }
        }

        if (Input.IsActionPressed("inputUp"))
        {
            if (!canAttack) return;
            attackArea.GlobalRotation = Mathf.DegToRad(-90f);
            attackArea.Position = new Vector2(0, -6);
            attackDirection = attackDirections.Up;
        }
        if (Input.IsActionPressed("inputLeft") && !isOnWall)
        {
            mesh.RotationDegrees = Mathf.Lerp(mesh.RotationDegrees, -5, 0.1f);
            player.healthBar.RotationDegrees = Mathf.Lerp(player.healthBar.RotationDegrees, -5, 0.1f);
            if (!canAttack) return;
            attackArea.GlobalRotation = Mathf.DegToRad(180f);
            attackArea.Position = Vector2.Zero;
            attackArea.Position = new Vector2(-6, 0);
            attackDirection = attackDirections.Left;
        }
        if (Input.IsActionPressed("inputRight") && !isOnWall)
        {
            mesh.RotationDegrees = Mathf.Lerp(mesh.RotationDegrees, 5, 0.1f);
            player.healthBar.RotationDegrees = Mathf.Lerp(player.healthBar.RotationDegrees, 5, 0.1f);
            if (!canAttack) return;
            attackArea.GlobalRotation = 0;
            attackArea.Position = Vector2.Zero;
            attackArea.Position = new Vector2(6, 0);
            attackDirection = attackDirections.Right;
        }
        if (Input.IsActionPressed("inputDown"))
        {
            if (!canAttack) return;
            attackArea.GlobalRotation = Mathf.DegToRad(90f);
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
                TargetVelocity.Y = JumpStrength;
            }
            healthComponent.TakeDamage(10);
        }
    }

    #endregion
    #region Movement
    public Vector2 TargetVelocity = Vector2.Zero;
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
    public float HorizontalInput;
    private void Movement(double delta)
    {
        isOnWall = player.IsOnWallOnly();
        HorizontalInput = Input.GetAxis("inputLeft", "inputRight");
        if (player.IsOnCeiling() && TargetVelocity.Y < 0) TargetVelocity.Y = Mathf.Lerp(TargetVelocity.Y, 0, 0.1f);
        if (isOnWall && TargetVelocity.X != 0) TargetVelocity.X = Mathf.Lerp(TargetVelocity.X, 0, 0.1f);
        if (!canWallDrag) HorizontalInput = 0;
        if (HorizontalInput == 0)
        {
            mesh.Rotation = Mathf.Lerp(mesh.Rotation, 0, 0.1f);
            player.healthBar.Rotation = Mathf.Lerp(player.healthBar.Rotation, 0, 0.1f);
        }
        if (HorizontalInput > 0)
        {
            player.Sprite.FlipH = false;
        }
        else if (HorizontalInput < 0)
        {
            player.Sprite.FlipH = true;
        }
        VelocityChange = 0;
        if (player.IsOnFloor())
        {
            if (HorizontalInput == 0)
            {
                VelocityChange = GroundDeceleration;
                MaxSpeed = 220;
            }
            else
            {
                VelocityChange = Acceleration;
                MaxSpeed = 220;
            }
        }
        else
        {
            if (HorizontalInput == 0)
            {
                VelocityChange = AirDeceleration;
            }
            else
            {
                VelocityChange = AirAcceleration;
                MaxSpeed = 250;
            }
        }
        TargetVelocity.X = Mathf.MoveToward(TargetVelocity.X, HorizontalInput * MaxSpeed, VelocityChange);
        if (Input.IsActionJustPressed("inputDash")) DashStart();
        if (player.IsOnFloor())
        {
            jumpCount = 0;
            TargetVelocity.Y = 0;
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
        player.Velocity = TargetVelocity;
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
    public void Jump()
    {
        if (isDashing || isOnWall) ResetGravity();
        TargetVelocity.Y += JumpStrength;
        jumpCount++;
    }
    private void WallJump()
    {
        ResetGravity();
        wallJumpTimer.Start();
        canWallDrag = false;
        TargetVelocity.Y += JumpStrength -30;
        TargetVelocity.X += CheckWallDirection() * 500;
        jumpCount = 0;
        jumpCount++;
    }
    private void DashStart()
    {
        if (!canDash) return;
        isDashing = true;
        canDash = false;
        var localHorizontalInput = Mathf.RoundToInt(HorizontalInput);
        if (localHorizontalInput == 0)
        {
            DashEnd();
            dashCooldownTimer.Start();
            return;
        }
        mesh.RotationDegrees = localHorizontalInput * 45f;
        player.healthBar.RotationDegrees = localHorizontalInput * 45f;
        TargetVelocity.X = localHorizontalInput * 900;
        TargetVelocity.Y = 0;
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
            var localHorizontalInput = Mathf.RoundToInt(HorizontalInput);
            if (localHorizontalInput != CheckWallDirection()) localGravity = Mathf.Lerp(0, 40f, 0.05f);
            if (TargetVelocity.Y != 0) TargetVelocity.Y = Mathf.Lerp(TargetVelocity.Y, 0, 0.1f);
        }
        else if (localGravity != 40 && gravityTimer.IsStopped()) ResetGravity();
        TargetVelocity.Y += localGravity;
        if (previousYVelocity < 0.01 && TargetVelocity.Y > 0)
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
            if (TargetVelocity.Y < 0) TargetVelocity.Y = -TargetVelocity.Y / 2;
        }
        TargetVelocity = new Vector2(TargetVelocity.X, Mathf.Min(TargetVelocity.Y, TerminalVelocity));
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
