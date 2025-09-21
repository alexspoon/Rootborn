using Godot;
using System;

public partial class PlayerWalking : PlayerState
{
    public PlayerWalking(Player plr, PlayerController ctrl) : base(plr, ctrl) { }

    public override void PhysicsProcess(float delta)
    {
        controller.VelocityChange = controller.Acceleration;
    }

}
