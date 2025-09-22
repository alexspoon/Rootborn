using Godot;
using System;

public partial class AttackUp : PlayerAttackState
{
    public AttackUp(Player plr, PlayerMoveComponent ctrl, Area2D atk) : base(plr, ctrl, atk) { }
    public override void Enter(State previous = null)
    {
        attackArea.GlobalRotation = Mathf.DegToRad(-90f);
        attackArea.Position = new Vector2(0, -6);
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
