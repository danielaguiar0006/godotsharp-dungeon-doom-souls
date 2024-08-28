using Godot;
using StatsAndAttributes;

public partial class Weapon : Item
{
    protected Mob m_Owner; // The Mob that owns this weapon, such as the player or an enemy
    protected WeaponStats m_Stats = new WeaponStats();
    protected Area3D m_HitBox;
    protected Animation m_Animation;
    protected bool m_IsCurrentlyEquipped;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        m_IsEquippable = true;
        m_IsPickupable = true;
        m_IsDroppable = true;
        m_IsCurrentlyEquipped = false;
    }

    public void Equip(Mob owner)
    {
        m_Owner = owner;
        m_IsCurrentlyEquipped = true;
    }

    public void Unequip()
    {
        m_Owner = null;
        m_IsCurrentlyEquipped = false;
    }

    public void AttackLight()
    {
        //TODO: attackstate length
    }
}

