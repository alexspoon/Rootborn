using Godot;

public partial class LevelManager : Node
{
    private Node _main;
    private string[] _savedLevels;
    private string _savedLevelsPath;
    private Level _currentLevel;

    public override void _Ready()
    {
        GetReferences();
        Init();
        FetchSavedLevels();
    }

    public void FetchSavedLevels()
    {
        _savedLevels = DirAccess.GetFilesAt(_savedLevelsPath);
    }

    private void Init()
    {
        _savedLevelsPath = "res://scenes/levels/";
    }

    private void GetReferences()
    {
        _main = GetTree().GetRoot().GetNode("Main");
    }

    public void SwitchLevel(string levelToLoad, bool mainMenu)
    {
        if (!ResourceLoader.Exists(_savedLevelsPath + levelToLoad + ".tscn"))
        {
            GD.PrintErr("Level " + levelToLoad + " does not exist.");
            return;
        }
        if (mainMenu)
        {
            var menu = _main.GetChild<MainMenu>(0);
            _main.CallDeferred(Node.MethodName.RemoveChild, menu);
        }
        else
        {
            _currentLevel = _main.GetChildOrNull<Level>(0);
            if (_currentLevel != null)
            {
               _main.CallDeferred(Node.MethodName.RemoveChild, _currentLevel);
            }
        }
        
        var levelScene = GD.Load<PackedScene>(_savedLevelsPath + levelToLoad + ".tscn");
        var levelInstance = levelScene.Instantiate<Level>();
        _main.CallDeferred(Node.MethodName.AddChild, levelInstance);
    }
}