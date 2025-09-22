using Godot;

public partial class PlayerMoveComponent : Node
{
    public Player player;
    public override void _Ready()
    {
        player = GetParent<Player>();
        AddMovementStates();
        AddAttackStates();
    }

    public override void _PhysicsProcess(double delta)
    {
        Movement(delta);
    }

    #region Movement
    #region MovementStats
    [ExportSubgroup("Movement Properties")]
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
    public float HorizontalInput;
    public bool InputLocked;
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
        if (!InputLocked) HorizontalInput = Input.GetAxis("inputLeft", "inputRight");
        else HorizontalInput = 0;
        TargetVelocity.X = Mathf.MoveToward(TargetVelocity.X, HorizontalInput * MaxSpeed, VelocityChange);
        player.Velocity = TargetVelocity;
        player.MoveAndSlide();
    }
    #endregion
    #region Attacks
    public FiniteStateMachine AttackStateMachine = new();

    private void AddAttackStates()
    {
        AttackStateMachine.AddState("Idle", new AttackIdle(player, this));
        AttackStateMachine.AddState("Up", new AttackUp(player, this));
        AttackStateMachine.AddState("Left", new AttackLeft(player, this));
        AttackStateMachine.AddState("Down", new AttackDown(player, this));
        AttackStateMachine.AddState("Right", new AttackRight(player, this));
        AttackStateMachine.ChangeState("Idle");
    }
    #endregion
}