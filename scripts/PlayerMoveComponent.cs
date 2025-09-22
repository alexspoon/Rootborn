using Godot;

public partial class PlayerMoveComponent : Node
{
    public Player player;
    public override void _Ready()
    {
        player = GetParent<Player>();
        AddMovementStates();
        AddAttackStates();

        //debug
        AttackStateMachine.DebugPrints = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        Movement(delta);
        Attacks(delta);
    }

    #region Movement
    #region MovementStats
    [ExportSubgroup("Movement Stats")]
    [Export] public bool SnapInput = true;
    [Export] public float MaxSpeed = 220f;
    [Export] public float GroundAcceleration = 50f;
    [Export] public float AirAcceleration = 75f;
    [Export] public float GroundDeceleration = 20f;
    [Export] public float AirDeceleration = 5f;
    [Export] public float JumpStrength = -520f;
    [Export] public float FallAceleration = 40f;
    [Export] public float TerminalVelocity = 400f;
    [Export] public float JumpEndEarlyGravityModifier = 3f;
    [Export] public float CoyoteTimeDuration = .1f;
    [Export] public float JumpBufferTime = .2f;
    [Export] public int MaxJumps = 1;
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 0.5f;
    #endregion
    public float VelocityChange;
    public Vector2 TargetVelocity = Vector2.Zero;
    public Vector2 VectorInput;
    public float HorizontalInput;
    public bool InputLocked;
    public float LastHorizontalInput = 1;
    public FiniteStateMachine MovementStateMachine = new();
    private void AddMovementStates()
    {
        MovementStateMachine.AddState("Idle", new MovementIdle(player, this));
        MovementStateMachine.AddState("Walking", new MovementWalking(player, this));
        MovementStateMachine.AddState("Midair", new MovementMidair(player, this));
        MovementStateMachine.AddState("Jump", new MovementJump(player, this));
        MovementStateMachine.AddState("WallGrab", new MovementWallGrab(player, this));
        MovementStateMachine.AddState("WallJump", new MovementWallJump(player, this));
        MovementStateMachine.AddState("Dash", new MovementDash(player, this));
        MovementStateMachine.ChangeState("Idle");
    }
    private void Movement(double delta)
    {
        MovementStateMachine.ExecuteStatePhysics((float)delta);
        VectorInput = Input.GetVector("inputLeft", "inputRight", "inputDown", "inputUp");
        if (!InputLocked) HorizontalInput = Input.GetAxis("inputLeft", "inputRight");
        if (SnapInput && HorizontalInput != 0)
        {
            if (HorizontalInput > 0) HorizontalInput = 1;
            else HorizontalInput = -1;
        }
        if (!InputLocked && Mathf.RoundToInt(HorizontalInput) != 0) LastHorizontalInput = Mathf.RoundToInt(HorizontalInput);
        else HorizontalInput = 0;
        TargetVelocity.X = Mathf.MoveToward(TargetVelocity.X, HorizontalInput * MaxSpeed, VelocityChange);
        player.Velocity = TargetVelocity;
        player.MoveAndSlide();
    }
    #endregion
    #region Attacks
    #region AttackStats
    [ExportSubgroup("Attack Stats")]
    [Export] public float AttackCooldown = 0.1f;
    [Export] public float AttackDamage = 10f;
    #endregion
    public FiniteStateMachine AttackStateMachine = new();
    public Area2D AttackArea;
    private void AddAttackStates()
    {
        AttackArea = player.GetNode<Area2D>("AttackArea");
        AttackStateMachine.AddState("Idle", new AttackIdle(player, this, AttackArea));
        AttackStateMachine.AddState("Up", new AttackUp(player, this, AttackArea));
        AttackStateMachine.AddState("Left", new AttackLeft(player, this, AttackArea));
        AttackStateMachine.AddState("Down", new AttackDown(player, this, AttackArea));
        AttackStateMachine.AddState("Right", new AttackRight(player, this, AttackArea));
        AttackStateMachine.ChangeState("Idle");
    }

    private void Attacks(double delta)
    {
        AttackStateMachine.ExecuteStatePhysics((float)delta);
    }
    #endregion
}