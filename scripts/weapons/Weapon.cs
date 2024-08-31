using Godot;
using Game.StatsManager;

public partial class Weapon : Item
{
    protected WeaponStats m_Stats = new WeaponStats();
    protected Area3D m_HitBox;
    protected Animation m_Animation;


    public Weapon()
    {
        SetIsEquippable(true);
        SetIsPickupable(true);
        SetIsDroppable(true);
        // m_HitBox = GetNode<Area3D>("HitBox");
        // m_Animation = GetNode<Animation>("Animation");
    }

    public override void _Ready()
    {
    }

    public void EquipState(ref Mob owner)
    {
        SetOwner(owner);
    }

    public void UnequipState()
    {
        SetOwner(null);
    }

    public virtual void AttackLightState()
    {
        // m_Owner.Attack(m_Stats.GetLightAttackDamage(), );
        //TODO: attackstate length
    }
    public virtual void AttackHeavyState()
    {
        //TODO: attackstate length
    }
    public virtual void AttackSpeacialState()
    {
        //TODO: attackstate length
    }
}

