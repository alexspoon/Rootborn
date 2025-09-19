using Godot;

public partial class PlayerCamera : Camera2D
{
    [ExportSubgroup("Camera Properties")]
    [Export] public float LerpWeight;
    [Export] private bool _debug;
    private MetaprogressionManager metaprogressionManager;
    private CharacterBody2D _player;
    public bool UsingGamepad = false;

    public override void _Ready()
    {
        _player = GetParent().GetNodeOrNull<CharacterBody2D>("Player");
        metaprogressionManager = GetNode<MetaprogressionManager>("/root/MetaprogressionManager");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey or InputEventMouse) UsingGamepad = false;
        else if (@event is InputEventJoypadButton or InputEventJoypadMotion) UsingGamepad = true;
    }


    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;
        if (!IsInstanceValid(_player)) return;
        GlobalPosition = new Vector2(Mathf.Lerp(GlobalPosition.X, _player.GlobalPosition.X, LerpWeight), Mathf.Lerp(GlobalPosition.Y, _player.GlobalPosition.Y, LerpWeight));

        switch (UsingGamepad)
        {
            case false:
                if (Input.IsActionJustPressed("inputZoomIn"))
                {
                    Zoom += new Vector2(0.1f, 0.1f);
                }
                if (Input.IsActionJustPressed("inputZoomOut"))
                {
                    var newZoom = Zoom - new Vector2(0.1f, 0.1f);
                    switch (_debug)
                    {
                        case true:
                            if (newZoom <= new Vector2(0.1f, 0.1f)) newZoom = new Vector2(0.1f, 0.1f);
                            break;
                        case false:
                            if (newZoom <= new Vector2(0.4f, 0.4f)) newZoom = new Vector2(0.4f, 0.4f);
                            break;
                    }
                    Zoom = newZoom;
                }
                break;
            case true:
                if (Input.IsActionPressed("inputZoomIn"))
                    {
                        Zoom += new Vector2(0.1f, 0.1f);
                    }
                if (Input.IsActionPressed("inputZoomOut"))
                    {
                        var newZoom = Zoom - new Vector2(0.1f, 0.1f);
                        switch (_debug)
                        {
                            case true:
                                if (newZoom <= new Vector2(0.1f, 0.1f)) newZoom = new Vector2(0.1f, 0.1f);
                                break;
                            case false:
                                if (newZoom <= new Vector2(0.4f, 0.4f)) newZoom = new Vector2(0.4f, 0.4f);
                                break;
                        }
                        Zoom = newZoom;
                    }
                break;
        }

        if (Input.IsActionJustPressed("inputZoomReset"))
        {
            Zoom = Vector2.One;
        }
    }
}
