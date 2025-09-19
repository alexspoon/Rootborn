using Godot;

public partial class PlayerHealthComponent : Node
{
    private Player _player;
    public float Health;

    public void Init()
    {
        _player = GetParent<Player>();
        Health = _player.playerStats.MaxHealth;
    }
    public void RecieveHealing(float healing)
    {
        Health += healing;
        if (Health > _player.playerStats.MaxHealth) Health = _player.playerStats.MaxHealth;
    }

    public void Death()
    {
        QueueFree();
    }

}
