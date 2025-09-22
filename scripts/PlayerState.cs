using Godot;
using System;

public partial class PlayerState : State
{
    protected Player player;
    protected PlayerMoveComponent controller;
    public PlayerState(Player plr, PlayerMoveComponent ctrl) { player = plr; controller = ctrl; }
}
