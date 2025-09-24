using Godot;
using System;

public partial class MovementJump : PlayerState
{
    public MovementJump(Player plr, PlayerMoveComponent ctrl) : base(plr, ctrl) { }
    private GpuParticles2D particleTrail;
    public State Previous;
    public override void Enter(State previous = null)
    {
        Previous = previous;
        particleTrail = controller.particleTrail;
        particleTrail.Set(ParticleProcessMaterial.PropertyName.InheritVelocityRatio, 1f);
        controller.TargetVelocity.Y = controller.JumpStrength;
    }

    public override void Exit()
    {
        particleTrail.Set(ParticleProcessMaterial.PropertyName.InheritVelocityRatio, -0.02f);
    }

    public override void PhysicsProcess(float delta)
    {
        StateMachine.ChangeState("Midair");
    }

}
