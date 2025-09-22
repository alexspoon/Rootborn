using Godot;
using System;
using System.Data;

public partial class AttackIdle : PlayerAttackState
{
    public AttackIdle(Player plr, PlayerMoveComponent ctrl, Area2D atk) : base(plr, ctrl, atk) { }

    public override void PhysicsProcess(float delta)
    {
        if (!Input.IsActionJustPressed("inputPrimaryAttack")) return;
        var movementState = controller.MovementStateMachine.CurrentState;
        switch (movementState)
        {
            case MovementIdle:
                if (Mathf.RoundToInt(controller.VectorInput.Y) == 1) StateMachine.ChangeState("Up");
                else if (controller.LastHorizontalInput == 1) StateMachine.ChangeState("Right");
                else StateMachine.ChangeState("Left");
                break;
            case MovementWalking:
                if (Mathf.RoundToInt(controller.VectorInput.Y) == 1) StateMachine.ChangeState("Up");
                else if (controller.HorizontalInput > 0) StateMachine.ChangeState("Right");
                else StateMachine.ChangeState("Left");
                break;
            case MovementMidair:
                if (Mathf.RoundToInt(controller.VectorInput.Y) == 1) StateMachine.ChangeState("Up");
                else if (Mathf.RoundToInt(controller.VectorInput.Y) == -1) StateMachine.ChangeState("Down");
                else if (controller.HorizontalInput > 0) StateMachine.ChangeState("Right");
                else StateMachine.ChangeState("Left");
                break;
        }
    }

}
