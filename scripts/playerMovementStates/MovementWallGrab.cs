using Godot;
using System;
using System.Threading.Tasks;

public partial class MovementWallGrab : PlayerState
{
    public MovementWallGrab(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    public int WallDirection;

    public override void Enter(State previous = null)
    {
        controller.TargetVelocity.Y = Mathf.Lerp(controller.TargetVelocity.Y, 0, 0.75f);
    }

    public override void PhysicsProcess(float delta)
    {
        player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, 0, 0.3f);
        WallDirection = CheckWallDirection();
        var slideSpeed = Mathf.Lerp(0, controller.FallAceleration, 0.1f);
        controller.TargetVelocity.Y += slideSpeed;
        controller.TargetVelocity = new Vector2(controller.TargetVelocity.X, Mathf.Min(controller.TargetVelocity.Y, controller.TerminalVelocity));
        if (Input.IsActionJustPressed("inputJump")) StateMachine.ChangeState("WallJump");
        else if (player.IsOnFloor())
        {
            if (controller.HorizontalInput == 0) StateMachine.ChangeState("Idle");
            else StateMachine.ChangeState("Walking");
        }
        else if (Mathf.RoundToInt(controller.HorizontalInput) == WallDirection || !player.IsOnWall()) StateMachine.ChangeState("Midair");
        else if (Input.IsActionJustPressed("inputDash")) StateMachine.ChangeState("Dash");
        
    }

    private int CheckWallDirection()
    {
        for (int i = 0; i < player.GetSlideCollisionCount(); i++)
        {
            var collision = player.GetSlideCollision(i);
            if (collision.GetNormal().X < 0) return -1;
            else if (collision.GetNormal().X > 0) return 1;
        }
        return 0;
    }
}
