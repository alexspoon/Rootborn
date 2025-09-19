using Godot;
[GlobalClass]

public partial class PlayerStats : Resource
{
    [Export] public float MaxHealth;
    [Export] public float BonusFlatMaxHealth;
    [Export] public float BonusPercentageMaxHealth;
    [Export] public float Toughness;
    [Export] public float BonusFlatToughness;
    [Export] public float BonusPercentageToughness;
    [Export] public float BonusFlatPrimaryDamage;
    [Export] public float BonusPercentagePrimaryDamage;
    [Export] public float BonusFlatSecondaryDamage;
    [Export] public float BonusPercentageSecondaryDamage;
    [Export] public float PickupRange;
}
