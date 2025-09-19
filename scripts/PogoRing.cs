using Godot;
using System;

public partial class PogoRing : Enemy
{
    private AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        healthComponent.OnDamage += Animate;
    }

    private void Animate()
    {
        animationPlayer.Play("Hit");
    }
}
