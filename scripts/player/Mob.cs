using Godot;
using Game.StatsAndAttributes;
using Game.DamageSystem;

public partial class Mob : CharacterBody3D
{
    public enum MobType
    {
        Player,
        NPC,
        Monster,
        Boss
    }

    protected MobType m_MobType;
    public MobStats m_Stats = new MobStats();
    protected bool m_IsAlive = true;
    protected bool m_IsHostile = true;


    public void TakeDamage(ref Damage damage)
    {
        float currentHealth = m_Stats.GetCurrentBaseStatValues()[BaseStatType.Health];

        // Apply damage resistance
        var resistanceFactors = m_Stats.GetResistanceAmountFactors();
        foreach (var damageTypeToDamageAmount in damage.m_DamageTable)
        {
            if (resistanceFactors.TryGetValue(damageTypeToDamageAmount.Key, out float resistanceFactor))
            {
                float damageAmount = damageTypeToDamageAmount.Value * resistanceFactor;
                currentHealth -= damageAmount;
            }
            else
            {
                // If no resistance factor is found, apply full damage
                currentHealth -= damageTypeToDamageAmount.Value;
            }
        }

        // TODO: Apply Special effects

        // Apply updated health
        m_Stats.SetCurrentBaseStatValue(BaseStatType.Health, currentHealth);
        GD.Print("Player Health: " + m_Stats.GetCurrentBaseStatValues()[BaseStatType.Health]);

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    // The target Mob is figured out in the AttackState
    // NOTE: We pass a ref to damageTable so that the weapon can pass different damage values to the mob depending on the type of attack (light, heavy, special, etc...)
    public void Attack(Damage damage, Mob target)
    {
        if (!m_IsAlive)
        {
            return;
        }

        if (target == null || !target.m_IsAlive)
        {
            return;
        }

        // Apply Attribute effects
        var attributeLevels = m_Stats.GetCurrentAttributeLevels();
        foreach (var damageTypeToDamageAmount in damage.m_DamageTable)
        {
            if (damageTypeToDamageAmount.Key == DamageType.Physical)
            {
                damage.m_DamageTable[DamageType.Physical] = damageTypeToDamageAmount.Value * (attributeLevels[AttributeType.Strength] * m_Stats.GetLevelEffectFactor());
            }
        }

        // Apply special effects
        var specialStats = m_Stats.GetSpecialStatAmountFactors();
        foreach (var damageTypeToDamageAmount in damage.m_DamageTable)
        {
            if (damageTypeToDamageAmount.Key == DamageType.Physical)
            {
                RandomNumberGenerator rng = new RandomNumberGenerator();
                if (specialStats[SpecialStatType.CritChance] >= rng.Randf())
                {
                    damage.m_DamageTable[DamageType.Physical] = damageTypeToDamageAmount.Value * specialStats[SpecialStatType.CritDamageFactor];
                }
            }
        }

        // Apply damage to the target - by sending a ref to damage object, which we potentially modified
        target.TakeDamage(ref damage);
    }

    public virtual void Die()
    {
        m_IsAlive = false;
        GD.Print("Mob has died");
    }

    // TODO: GETTERS AND SETTERS
    public MobType GetMobType()
    {
        return m_MobType;
    }

    // public MobStats GetStats()
    // {
    //     return m_Stats;
    // }

    public void SetIsHostile(bool isHostile)
    {
        m_IsHostile = isHostile;
    }
}
