using System.Collections.Generic;
using Game.DamageSystem;

namespace Game.StatsManager
{
    public enum BaseStatType
    {
        Health,
        Mana,
        Stamina,
        EquipWeight,
        Souls
    }

    public enum AttributeType
    {
        Vigor,
        Mind,
        Endurance,
        Strength,
        Dexterity,
        Intelligence,
        Faith,
        Charisma
    }

    public enum SpecialStatType
    {
        MovementSpeedFactor,
        SprintSpeedFactor,
        DodgeSpeedFactor,
        // TODO: JumpHeightFactor,
        CritChance,
        CritDamageFactor,
        BlockPhysicalAmount,
        // MagicCritChance,
        // MagicCritDamage,
        // MagicBlockAmount,
        // HealthRegen,
        // ManaRegen,
        // StaminaRegen,
        // EquipWeightModifier,
        // Luck
    }

    public enum WeaponType
    {
        Melee,
        HitScan,
        Projectile
    }

    public class MobStats
    {
        // Every level, adds 10% effectiveness if applicable
        public float m_LevelEffectFactor { get; private set; } = 1.1f;

        // Max Base Stat Amounts
        public Dictionary<BaseStatType, float> m_BaseStatTypeToMaxValue { get; private set; } = new Dictionary<BaseStatType, float>() {
            { BaseStatType.Health, 100.0f },
            { BaseStatType.Mana, 100.0f },
            { BaseStatType.Stamina, 100.0f },
            { BaseStatType.EquipWeight, 25.0f }
        };
        // Current Base Stat Amounts
        public Dictionary<BaseStatType, float> m_BaseStatTypeToCurrentValue { get; private set; } = new Dictionary<BaseStatType, float>() {
            { BaseStatType.Health, 100.0f },
            { BaseStatType.Mana, 100.0f },
            { BaseStatType.Stamina, 100.0f },
            { BaseStatType.EquipWeight, 0.0f }
        };

        // Mob Attribute Levels
        public Dictionary<AttributeType, int> m_AttributeTypeToCurrentLevel { get; private set; } = new Dictionary<AttributeType, int>() {
            { AttributeType.Vigor, 1 },
            { AttributeType.Mind, 1 },
            { AttributeType.Endurance, 1 },
            { AttributeType.Strength, 1 },
            { AttributeType.Dexterity, 1 },
            { AttributeType.Intelligence, 1 },
            { AttributeType.Faith, 1 },
            { AttributeType.Charisma, 1 }
        };

        // Mob Special Stats - Special stats are modifiers with different effects and implementations
        public Dictionary<SpecialStatType, float> m_SpecialStatTypeToAmountFactor { get; private set; } = new Dictionary<SpecialStatType, float>() {
            // NOTE: Special stats are implemented in completely different ways, so check specific implemntation before editing values

            // ------ Movement ----------------------------
            { SpecialStatType.MovementSpeedFactor, 1.0f }, // Multiplier of movement speed, this affects most movement by default
                                                           // (1.0f = regular movement speed ; 2.0f = double movemnet speed).
            { SpecialStatType.SprintSpeedFactor, 1.75f },  // Multiplier of movement speed when sprinting (1.0f = regular movement
                                                           // speed ; 2.0f = double movement speed).
            { SpecialStatType.DodgeSpeedFactor, 1.0f },    // Multiplier of dodge speed (1.0f = regular dodge speed ; 2.0f = double dodge speed)
            // --------------------------------------------

            // ------ Physical Damage ---------------------
            { SpecialStatType.CritChance, 0.1f },          // Min: 0.0f, Max: 1.0f (0% - 100%)
            { SpecialStatType.CritDamageFactor, 1.5f },    // Multiplier of damage on crit (2.0f = 2x more damage on crit)
            { SpecialStatType.BlockPhysicalAmount, 0.5f }, // Amount of physical damage blocked when blocking (0.5f = 50% damage reduction)
            // --------------------------------------------

            // ------ Magic ------
            // { SpecialStatType.MagicCritChance, 1.0f },
            // { SpecialStatType.MagicCritDamage, 1.0f },
            // { SpecialStatType.MagicBlockAmount, 1.0f },
            // -------------------

            // { SpecialStatType.HealthRegenFactor, 1.0f },
            // { SpecialStatType.ManaRegenFactor, 1.0f },
            // { SpecialStatType.StaminaRegenFactor, 1.2f },
            // { SpecialStatType.EquipWeightFactor, 1.0f },
            // { SpecialStatType.Luck, 1.0f }
        };

        // Mob Resistances - Resistances are multipliers that reduce damage taken from specific damage types
        // NOTE: This means 2.0f = 2x more damage taken, 0.5f = 50% less damage taken
        public Dictionary<DamageType, float> m_DamageTypeToResistanceAmountFactor { get; private set; } = new Dictionary<DamageType, float>() {
            { DamageType.Physical, 1.0f },
            { DamageType.Fire, 1.0f },
            { DamageType.Ice, 1.0f },
            { DamageType.Lightning, 1.0f }
        };

        // Setters
        public void SetMaxBaseStatValue(BaseStatType baseStatType, float amount) { m_BaseStatTypeToMaxValue[baseStatType] = amount; }
        public void SetCurrentBaseStatValue(BaseStatType baseStatType, float amount) { m_BaseStatTypeToCurrentValue[baseStatType] = amount; }
        public void SetCurrentAttributeLevel(AttributeType attributeType, int level) { m_AttributeTypeToCurrentLevel[attributeType] = level; }
        public void SetSpecialStatAmountFactor(SpecialStatType specialStatType, float amount) { m_SpecialStatTypeToAmountFactor[specialStatType] = amount; }
        public void SetResistanceAmountFactor(DamageType damageType, float amount) { m_DamageTypeToResistanceAmountFactor[damageType] = amount; }
    }

    public class WeaponStats
    {
        public int m_WeaponLevel { get; private set; } = 1;
        public WeaponType m_WeaponType { get; private set; }
        public Damage m_LightAttackDamage { get; private set; }
        public Damage m_HeavyAttackDamage { get; private set; }
        public Damage m_SpecialAttackDamage { get; private set; }

        // Setters
        public void SetWeaponLevel(int level) { m_WeaponLevel = level; }
        public void SetWeaponType(WeaponType weaponType) { m_WeaponType = weaponType; }
        public void SetLightAttackDamage(Damage damage) { m_LightAttackDamage = damage; }
        public void SetHeavyAttackDamage(Damage damage) { m_HeavyAttackDamage = damage; }
        public void SetSpecialAttackDamage(Damage damage) { m_SpecialAttackDamage = damage; }
    }
}
