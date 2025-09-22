using Godot;
using System;

public partial class MovementWalking : PlayerState
{
    public MovementWalking(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }

    public override void PhysicsProcess(float delta)
    {
        player.Mesh.RotationDegrees = Mathf.Lerp(player.Mesh.RotationDegrees, Mathf.RoundToInt(controller.HorizontalInput) * 15f, 0.25f);
        controller.VelocityChange = controller.GroundAcceleration;
        controller.TargetVelocity.Y = 0;
        if (Input.IsActionJustPressed("inputJump")) StateMachine.ChangeState("Jump");
        else if (Input.IsActionJustPressed("inputDash")) StateMachine.ChangeState("Dash");
        else if (!player.IsOnFloor()) StateMachine.ChangeState("Midair");
        else if (controller.HorizontalInput == 0) StateMachine.ChangeState("Idle");
    }

}
