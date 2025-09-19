using Godot;
[GlobalClass]

public partial class WeaponStats : Resource
{

    //PrimaryFire stats 
    [Export] public float PrimaryDamage;
    [Export] public float PrimaryCooldown;
    [Export] public bool PrimaryRequiresAmmo;
    [Export] public float PrimaryAmmoCount;
    [Export] public float PrimaryAmmoCost;
    [Export] public float PrimaryReloadTime;
    [Export] public bool PrimaryIsRoundsReload;
    [Export] public bool PrimaryIsMelee;

    //SecondaryFire stats
    [Export] public float SecondaryDamage;
    [Export] public float SecondaryCooldown;
    [Export] public bool SecondaryRequiresAmmo;
    [Export] public float SecondaryAmmoCount;
    [Export] public float SecondaryAmmoCost;
    [Export] public float SecondaryReloadTime;
    [Export] public bool SecondaryIsRoundsReload;
    [Export] public bool SecondaryIsMelee;
}
