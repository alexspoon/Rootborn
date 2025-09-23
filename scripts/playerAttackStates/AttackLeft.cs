using Godot;
using System;

public partial class AttackLeft : PlayerAttackState
{
    public AttackLeft(Player plr, PlayerMoveComponent ctrl, Area2D atk) : base(plr, ctrl, atk) { }
    public override void Enter(State previous = null)
    {
        attackArea.GlobalRotation = Mathf.DegToRad(180f);
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
        var attackSprite = attackArea.GetNode<AnimatedSprite2D>("AttackSprite");
        attackSprite.Play("Slash");
        Hitcheck();
    }

}
