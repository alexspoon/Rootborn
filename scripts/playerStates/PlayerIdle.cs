using Godot;
using System;

public partial class PlayerIdle : PlayerState
{
    public PlayerIdle(Player plr, PlayerController ctrl) : base(plr, ctrl) { }

    public override void Enter(State previous = null)
    {
        
    }


    public override void PhysicsProcess(float delta)
    {
        controller.VelocityChange = controller.GroundDeceleration;
    }

}
