using System;
using Godot;

public partial class MainMenu : Level
{
    private bool usingGamepad;
    private TextureButton _playButton;
    private TextureButton _configButton;
    private TextureButton _quitButton;
    private LevelManager _levelManager;
    private LineEdit _levelSelection;
    [Export] private string _defaultLevel = "TestingLevel";

    public override void _Ready()
    {
        _playButton = GetNode<TextureButton>("PlayButton");
        _configButton = GetNode<TextureButton>("ConfigButton");
        _quitButton = GetNode<TextureButton>("QuitButton");
        _levelSelection = GetNode<LineEdit>("LevelSelection");

        _levelManager = GetNode<LevelManager>("/root/LevelManager");

        _playButton.Pressed += PlayPressed;
        _configButton.Pressed += ConfigPressed;
        _quitButton.Pressed += QuitPressed;
    }

    public override void _Process(double delta)
    {
        if (!usingGamepad) return;
        var defaultFocusTexture = _playButton.TextureFocused;
        var previousFocus = _playButton;
        var focus = GetViewport().GuiGetFocusOwner();
        if (focus == null) return;
        if (focus != previousFocus)
        {
            previousFocus.TextureFocused = defaultFocusTexture;
        }
        if (Input.IsActionJustPressed("inputJump"))
        {
            var button = (TextureButton)focus;
            previousFocus = button;
            button.TextureFocused = button.TexturePressed;
        }
        if (Input.IsActionJustReleased("inputJump"))
        {
            var button = (TextureButton)focus;
            button.TextureFocused = defaultFocusTexture;
            button.EmitSignal(TextureButton.SignalName.Pressed);
        }
    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey or InputEventMouse) usingGamepad = false;
        else if (@event is InputEventJoypadButton or InputEventJoypadMotion && usingGamepad == false)
        {
            usingGamepad = true;
            _playButton.GrabFocus();
        }

        // if (@event is InputEventJoypadButton button && button.ButtonIndex == 0)
        // {
        //     var focus = GetViewport().GuiGetFocusOwner();
        //     if (focus is TextureButton)
        //     {
        //         // focus.EmitSignal(TextureButton.SignalName.ButtonDown);
        //         focus.GrabClickFocus();
        //     }
        // }
    }

    private void PlayPressed()
    {
        string levelName = _levelSelection.Text;
        if (levelName.Length == 0) levelName = _defaultLevel;
        _levelManager.SwitchLevel(levelName, true);
    }

    private void ConfigPressed()
    {
        
    }

    private void QuitPressed()
    {
        GetTree().Quit();
    }
}