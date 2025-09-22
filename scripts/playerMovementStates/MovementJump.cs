using Godot;
using System;

public partial class MovementJump : PlayerState
{
    public MovementJump(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    public bool Pogo = false;

    public override void Enter(State previous = null)
    {
        if (controller.AttackStateMachine.PreviousStateName == "Down") Pogo = true;
        controller.TargetVelocity.Y = controller.JumpStrength;
    }
    public override void PhysicsProcess(float delta)
    {
        StateMachine.ChangeState("Midair");
    }

}
