using Godot;
using Godot.Collections;

public partial class MetaprogressionManager : Node
{
    public Dictionary<MineableBlock.OreTypes, int> OreCount = new();
    public void AddOre(MineableBlock.OreTypes ore, int amount = 1)
    {
        if (!OreCount.ContainsKey(ore)) OreCount.Add(ore, 0);
        OreCount[ore] += amount;
    }
    public bool SpendOre(MineableBlock.OreTypes ore, int amount)
    {
        if (OreCount[ore] < amount)
        {
            return false;
        }
        OreCount[ore] -= amount;
        return true;
    }

    public override void _Ready()
    {
        GD.Print(OreCount);
    }
}
