using Godot;
using System;

public partial class AttackLeft : PlayerAttackState
{
    public AttackLeft(Player plr, PlayerMoveComponent ctrl, Area2D atk) : base(plr, ctrl, atk) { }
    public override void Enter(State previous = null)
    {
        attackArea.GlobalRotation = Mathf.DegToRad(180f);
        attackArea.Position = new Vector2(-6, 0);
        Hitcheck();
        var cooldown = UtilityFunctions.CreateOneShotTimer(controller.AttackCooldown, player);
        cooldown.Timeout += () =>
        {
            StateMachine.ChangeState("Idle");
            cooldown.QueueFree();
        };
    }

}
