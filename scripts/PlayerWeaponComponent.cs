using Godot;

public partial class PlayerWeaponComponent : Node
{
    public bool playerHasWeapon;
    private Node2D _weaponPivot;
    [Export] public Weapon playerWeapon;
    public bool weaponHeld;
    public override void _Ready()
    {
        _weaponPivot = GetParent().GetNode<Node2D>("WeaponPivot");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("inputEquipWeapon")) weaponHeld = !weaponHeld;
        playerHasWeapon = playerWeapon != null;
        if (playerHasWeapon)
        {
            var mousePos = _weaponPivot.GetGlobalMousePosition();
            playerWeapon.weaponSprite.Visible = weaponHeld;
            _weaponPivot.LookAt(mousePos);
            playerWeapon.weaponSprite.FlipV = mousePos.X < GetParent<CharacterBody2D>().GlobalPosition.X;
        }
        if (!weaponHeld) return;
        if (Input.IsActionPressed("inputPrimaryAttack")) playerWeapon.PrimaryFire();
        if (Input.IsActionPressed("inputSecondaryAttack")) playerWeapon.SecondaryFire();
    }
}
