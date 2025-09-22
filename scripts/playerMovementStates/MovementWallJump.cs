using Godot;
using System;

public partial class MovementWallJump : PlayerState
{
    public MovementWallJump(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    public override void Enter(State previous = null)
    {
        if (previous is MovementWallGrab)
        {
            var wallGrab = (MovementWallGrab)previous;
            controller.TargetVelocity.Y = controller.JumpStrength;
            controller.TargetVelocity.X = wallGrab.WallDirection * 500;
            player.Mesh.RotationDegrees = wallGrab.WallDirection * 30f;
            LockInput();
        }
    }

    public override void PhysicsProcess(float delta)
    {
        StateMachine.ChangeState("Midair");
    }

    private void LockInput()
    {
        controller.InputLocked = true;
        var lockTimer = UtilityFunctions.CreateOneShotTimer(0.025f, player);
        lockTimer.Timeout += () =>
        {
            controller.InputLocked = false;
            lockTimer.QueueFree();
        };
    }

}
