using Godot;
using System;

public partial class MovementJump : PlayerState
{
    public MovementJump(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }

    public override void Enter(State previous = null)
    {
        if (previous is not MovementDash) controller.TargetVelocity.Y = controller.JumpStrength;
    }
    public override void PhysicsProcess(float delta)
    {
        StateMachine.ChangeState("Midair");
    }

}
