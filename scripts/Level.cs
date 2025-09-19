using Godot;

[GlobalClass]
public partial class Level : Node
{
    [Export] public string LevelName;
    [Export] public string LevelDescription;
    [Export] public Texture2D LevelPreview;
    [Export] public bool SaveToFile;
    [Export] public bool Overwrite;

    public override void _Ready()
    {
        if (SaveToFile) Save();
    }

    private void Save()
    {
        var filepath = "res://scenes/levels/" + LevelName + ".tscn";
        if (ResourceLoader.Exists(filepath))
        {
            switch (Overwrite)
            {
                case true:
                    GD.Print("Deleting level " + LevelName);
                    DirAccess.RemoveAbsolute(filepath);
                    SaveToFile = false;
                    Overwrite = false;
                    break;
                case false:
                    GD.Print("Level " + LevelName + " not saved, since it already exists.");
                    return;
            }
        }
        var children = GetChildren();
        if (children == null) return;
        foreach (var child in children) child.Owner = this;
        var scene = new PackedScene();
        scene.Pack(this);
        ResourceSaver.Save(scene, filepath);
        GD.Print("Saved level " + LevelName + " to " + filepath);
    }
}