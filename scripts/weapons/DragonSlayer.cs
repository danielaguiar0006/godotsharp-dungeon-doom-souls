using Godot;
using Game.StatsManager;

public partial class DragonSlayer : Weapon
{
    DragonSlayer()
    {
        SetItemName("Dragon Slayer");
        SetItemSlotType(Item.ItemSlot.MainHand);
        m_WeaponStats.SetWeaponType(WeaponType.Melee);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

}
