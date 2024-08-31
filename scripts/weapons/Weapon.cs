using Godot;
using Game.StatsAndAttributes;

public partial class Weapon : Item
{
    protected WeaponStats m_Stats = new WeaponStats();
    protected Area3D m_HitBox;
    protected Animation m_Animation;


    public override void _Ready()
    {
        m_IsEquippable = true;
        m_IsPickupable = true;
        m_IsDroppable = true;
    }

    public void Equip(ref Mob owner)
    {
        base.m_Owner = owner;
    }

    public void Unequip()
    {
        base.m_Owner = null;
    }

    public virtual void AttackLight()
    {
        // m_Owner.Attack(m_Stats.GetLightAttackDamage(), );
        //TODO: attackstate length
    }
    public virtual void AttackHeavy()
    {
        //TODO: attackstate length
    }
    public virtual void AttackSpeacial()
    {
        //TODO: attackstate length
    }
}

