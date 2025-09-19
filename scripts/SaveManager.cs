using Godot.Collections;
using System.IO;
using System.Text.Json;
using Godot;

public partial class SaveManager : Node
{
    private MetaprogressionManager metaprogressionManager;
    public override void _Ready()
    {
        metaprogressionManager = GetNode<MetaprogressionManager>("/root/MetaprogressionManager");
    }
    public void SaveMetaprogression(string fileName)
    {
        var jsonString = JsonSerializer.Serialize(metaprogressionManager.OreCount);
        File.WriteAllText(fileName, jsonString);
    }
    public void LoadMetaprogression(string fileName)
    {
        var jsonString = File.OpenRead(fileName);
        var newData = JsonSerializer.Deserialize<Dictionary<MineableBlock.OreTypes, int>>(jsonString);
        metaprogressionManager.OreCount = newData;
        GD.Print(metaprogressionManager.OreCount);
        jsonString.Close();
    }
}
