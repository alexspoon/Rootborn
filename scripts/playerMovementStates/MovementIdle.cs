using Godot;
using System;

public partial class MovementIdle : PlayerState
{
    public MovementIdle(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    public override void PhysicsProcess(float delta)
    {
        player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, 0, 0.3f);
        controller.VelocityChange = controller.GroundDeceleration;
        controller.TargetVelocity.Y = 0;
        if (!player.IsOnFloor()) StateMachine.ChangeState("Midair");
        else if (Input.IsActionJustPressed("inputJump")) StateMachine.ChangeState("Jump");
        else if (controller.HorizontalInput != 0) StateMachine.ChangeState("Walking");
    }

}
