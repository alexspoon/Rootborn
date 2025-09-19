using System;
using Godot;
[GlobalClass]

public partial class HealthComponent : Node
{
    private Timer invulnerabilityTimer = new();
    public event Action OnDamage;
    public event Action OnDeath;
    [Export] public float MaxHealth;
    [Export] public float Health;
    [Export] public float Toughness;
    [Export] public float InvulnerabilityDuration = 0.2f;
    public bool Invulnerable = false;

    public override void _Ready()
    {
        FullHeal();
        invulnerabilityTimer.WaitTime = InvulnerabilityDuration;
        invulnerabilityTimer.OneShot = true;
        CallDeferred(MethodName.AddChild, invulnerabilityTimer);
        invulnerabilityTimer.Timeout += InvulnerabilityEnd;
        OnDamage += InvulnerabilityStart;
    }

    public virtual void TakeDamage(float damage)
    {
        if (damage < Toughness) damage = Toughness;
        Health -= damage - Toughness;
        GD.Print(Owner.Name + " " + Health + " health remaining");
        if (Health <= 0) Death();
        OnDamage?.Invoke();
    }

    public virtual void FullHeal()
    {
        Health = MaxHealth;
    }

    public virtual void RecieveHealing(float healing)
    {
        Health += healing;
        if (Health > MaxHealth) FullHeal();
    }

    public virtual void Death()
    {
        OnDeath?.Invoke();
        GetParent().QueueFree();
    }

    private void InvulnerabilityStart()
    {
        Invulnerable = true;
        invulnerabilityTimer.Start();
    }

    private void InvulnerabilityEnd()
    {
        Invulnerable = false;
    }
}
