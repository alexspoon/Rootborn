using Godot;
using System;

public partial class PlayerState : State
{
    protected Player player;
    protected PlayerController controller;
    public PlayerState(Player plr, PlayerController ctrl) { player = plr; controller = ctrl; }
}
