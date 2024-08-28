using Godot;
using Game.StatsAndAttributes;

public partial class DragonSlayer : Weapon
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        m_Stats.SetWeaponType(WeaponStats.WeaponType.Melee);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
