using Godot;
using System;

public partial class PlayerMidair : PlayerState
{
    public PlayerMidair(Player plr, PlayerController ctrl) : base(plr, ctrl) { }

    public override void Enter(State previous = null)
    {
        controller.MaxSpeed = 250;
    }

    public override void Exit()
    {
        controller.MaxSpeed = 220;
    }

    public override void PhysicsProcess(float delta)
    {
        if (controller.HorizontalInput == 0)
        {
            controller.VelocityChange = controller.AirDeceleration;
        }
        else controller.VelocityChange = controller.AirAcceleration;
    }
}
