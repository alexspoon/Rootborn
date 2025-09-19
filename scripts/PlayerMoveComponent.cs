using Godot;

public partial class PlayerMoveComponent : Node
{
    private Vector2 _targetVelocity;
    private bool _canJump;
    private Timer _coyoteTimer = new();
    private int _maxJumps = 1;
    [Export] private float _coyoteTime = 0.2f;
    [Export] private float _acceleration = 75f;
    [Export] private float _airAcceleration = 80f;
    [Export] private float _drag = 70f;
    [Export] private float _airDrag = 60f;
    [Export] private float _speed = 300f;
    [Export] private float _gravity = 1500f;
    [Export] private float _jumpVelocity = -500f;

    public override void _Ready()
    {
        _coyoteTimer.WaitTime = _coyoteTime;
        _coyoteTimer.OneShot = true;
        _coyoteTimer.Timeout += CoyoteTimeTimeout;
        AddChild(_coyoteTimer);
    }


    public void Movement(Player player, double delta)
    {
        int jumpCount = 0;
        var horizontalInput = Input.GetAxis("inputLeft", "inputRight");
        if (horizontalInput == 1)
        {
            player.Sprite.FlipH = false;
        }
        else if (horizontalInput == -1)
        {
            player.Sprite.FlipH = true;
        }
        var velocityChange = 0f;
        if (player.IsOnFloor())
        {
            if (horizontalInput == 0)
            {
                velocityChange = _drag;
            }
            else velocityChange = _acceleration;
        }
        else
        {
            if (horizontalInput == 0)
            {
                velocityChange = _airDrag;
            }
            else velocityChange = _airAcceleration;
        }
        _targetVelocity.X = Mathf.MoveToward(_targetVelocity.X, horizontalInput * _speed, velocityChange);
        if (player.IsOnFloor() || player.IsOnCeiling())
        {
            _targetVelocity.Y = 0;
        }
        if (player.IsOnFloor() && _canJump == false || player.IsOnWall() && _canJump == false)
        {
            _canJump = true;
            jumpCount = 0;
        }
        if (!player.IsOnFloor() && _coyoteTimer.IsStopped() && jumpCount < _maxJumps)
        {
            if (!player.IsOnWall())
            {
                _coyoteTimer.Start();
                GD.Print("coyotetime start");
            }
        }
        if (!player.IsOnFloor()) _targetVelocity.Y += _gravity * (float)delta;
        if (_canJump && jumpCount < _maxJumps)
        {
            if (Input.IsActionJustPressed("inputJump")) _targetVelocity.Y = _jumpVelocity;
            jumpCount++;
            if (jumpCount >= _maxJumps) _canJump = false;
        }
        player.Velocity = _targetVelocity;
        player.MoveAndSlide();

    }

    private void CoyoteTimeTimeout()
    {
        _canJump = false;
        GD.Print("coyotetime finish");
    }
}
