using Godot;

public partial class TimerParticles : GpuParticles2D
{
    [Export] public bool LinkedPosition;
    [Export] public bool LinkedRotation;
    [Export] public float ParticleLifetime;
    public Node2D LinkedObject;

    public override void _Ready()
    {
        LinkedObject = GetParent<Node2D>();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(LinkedObject)) return;
        if (LinkedPosition) GlobalPosition = LinkedObject.GlobalPosition;
        if (LinkedRotation) GlobalRotation = LinkedObject.GlobalRotation;
        if (LinkedObject.IsQueuedForDeletion())
        {
            StartTimer();
            Node main = GetTree().GetRoot();
            Reparent(main, true);
        }
    }

    private void StartTimer()
    {
        Emitting = false;
        var lifetime = GetTree().CreateTimer(ParticleLifetime);
        lifetime.Timeout += Timeout;
    }
    private void Timeout()
    {
        QueueFree();
    }
}
