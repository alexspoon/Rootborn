using Godot;
using System;

public partial class AttackRight : PlayerAttackState
{
    public AttackRight(Player plr, PlayerMoveComponent ctrl, Area2D atk) : base(plr, ctrl, atk) { }
    public override void Enter(State previous = null)
    {
        attackArea.GlobalRotation = 0;
        attackArea.Position = Vector2.Zero;
        var cooldown = UtilityFunctions.CreateOneShotTimer(controller.AttackCooldown, player);
        cooldown.Timeout += () =>
        {
            StateMachine.ChangeState("Idle");
            cooldown.QueueFree();
        };
    }
    public override void PhysicsProcess(float delta)
    {
        Hitcheck();
    }


}
