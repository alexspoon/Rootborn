using Godot;
using System;

public partial class PlayerJump : PlayerState
{
    public PlayerJump(Player plr, PlayerController ctrl) : base(plr, ctrl) { }

    public override void Enter(State previous = null)
    {
        controller.Jump();
    }

}
