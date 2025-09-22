using Godot;
using System;

public partial class MovementDash : PlayerState
{
    public MovementDash(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    private float initialInput;

    public override void Enter(State previous = null)
    {
        controller.InputLocked = true;
        initialInput = Mathf.RoundToInt(controller.HorizontalInput);
        if (previous is MovementWallGrab)
        {
            initialInput = -initialInput;
        }
        var dashTimer = UtilityFunctions.CreateOneShotTimer(controller.DashDuration, player);
        dashTimer.Timeout += () =>
        {
            if (!dashTimer.IsInsideTree()) return;
            CheckNextState();
            dashTimer.QueueFree();
        };
    }

    public override void Exit()
    {
        controller.InputLocked = false;
    }

    private void CheckNextState()
    {
        switch (player.IsOnFloor())
            {
                case true:
                    if (controller.HorizontalInput == 0) StateMachine.ChangeState("Idle");
                    else StateMachine.ChangeState("Walking");
                    break;
                case false:
                    if (player.IsOnWallOnly()) StateMachine.ChangeState("WallGrab");
                    else StateMachine.ChangeState("Midair");
                    break;
            }
    }

    public override void PhysicsProcess(float delta)
    {
        player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, initialInput * 45, 0.3f);
        controller.TargetVelocity.Y = Mathf.Lerp(controller.TargetVelocity.Y, 0, 0.5f);
        var t = 0.5f;
        float wacky = 3 * t * t - 2 * t * t * t;
        var dashSpeed = Mathf.Lerp(controller.TargetVelocity.X, initialInput * controller.MaxSpeed * 3, wacky);
        controller.TargetVelocity.X = dashSpeed;
        if (player.IsOnWallOnly())
        {
            CheckNextState();
        }
    }

}
