using Godot;
using System;

public partial class PlayerAttackState : State
{
    protected Player player;
    protected PlayerMoveComponent controller;
    protected Area2D attackArea;
    public PlayerAttackState(Player plr, PlayerMoveComponent ctrl, Area2D atk) { player = plr; controller = ctrl; attackArea = atk; }
    public bool Hitcheck()
    {
        foreach (var body in attackArea.GetOverlappingBodies())
        {
            if (body.GetNodeOrNull<HealthComponent>("HealthComponent") == null) return false;
            var healthComponent = body.GetNode<HealthComponent>("HealthComponent");
            if (healthComponent.Invulnerable) return false;
            healthComponent.TakeDamage(controller.AttackDamage);
            return true;
        } return false;
    }
}
