using Godot;
using Game.StatsManager;
using Game.DamageSystem;

public partial class Weapon : Item
{
    public WeaponStats m_WeaponStats = new WeaponStats();
    public Area3D m_HitBox { get; private set; }
    public Damage m_LightAttackDamage { get; private set; }
    public Damage m_HeavyAttackDamage { get; private set; }
    public Damage m_SpecialAttackDamage { get; private set; }
    public Animation m_LightAttackAnimation { get; private set; }
    public Animation m_HeavyAttackAnimatio { get; private set; }
    public Animation m_SpecialAttackAnimation { get; private set; }


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
    public virtual void AttackSpecialState()
    {
        //TODO: attackstate length
    }

    // -------------------------------------------
    // Setters
    // -------------------------------------------
    public void SetHitBox(Area3D hitBox) { m_HitBox = hitBox; }
    public void SetLightAttackDamage(Damage lightAttackDamage) { m_LightAttackDamage = lightAttackDamage; }
    public void SetHeavyAttackDamage(Damage heavyAttackDamage) { m_HeavyAttackDamage = heavyAttackDamage; }
    public void SetSpecialAttackDamage(Damage specialAttackDamage) { m_SpecialAttackDamage = specialAttackDamage; }
    public void SetLightAttackAnimation(Animation lightAttackAnimation) { m_LightAttackAnimation = lightAttackAnimation; }
    public void SetHeavyAttackAnimation(Animation heavyAttackAnimation) { m_HeavyAttackAnimatio = heavyAttackAnimation; }
    public void SetSpecialAttackAnimation(Animation specialAttackAnimation) { m_SpecialAttackAnimation = specialAttackAnimation; }
}

