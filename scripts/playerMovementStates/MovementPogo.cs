using Godot;
using System;

public partial class MovementPogo : PlayerState
{
    public MovementPogo(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    public override void Enter(State previous = null)
    {
        controller.TargetVelocity.Y = controller.JumpStrength;
    }
    public override void PhysicsProcess(float delta)
    {
        StateMachine.ChangeState("Midair");
    }

}
