using System.Collections.Generic;

namespace StatsAndAttributes
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
        SpeedMultiplier,
        CritChance,
        CritDamageMultiplier,
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

    public class MobStats
    {

        // Every level, adds 10% effectiveness if applicable
        public const float LEVEL_EFFECT_FACTOR = 1.1f;

        // Max Base Stat Amounts
        private Dictionary<BaseStatType, float> m_BaseStatTypeToMaxValue = new Dictionary<BaseStatType, float>() {
            { BaseStatType.Health, 100.0f },
            { BaseStatType.Mana, 100.0f },
            { BaseStatType.Stamina, 100.0f },
            { BaseStatType.EquipWeight, 25.0f }
        };
        // Current Base Stat Amounts
        private Dictionary<BaseStatType, float> m_BaseStatTypeToCurrentValue = new Dictionary<BaseStatType, float>() {
            { BaseStatType.Health, 100.0f },
            { BaseStatType.Mana, 100.0f },
            { BaseStatType.Stamina, 100.0f },
            { BaseStatType.EquipWeight, 0.0f }
        };

        // Mob Attribute Levels
        private Dictionary<AttributeType, int> m_AttributeTypeToCurrentLevel = new Dictionary<AttributeType, int>() {
            { AttributeType.Vigor, 1 },
            { AttributeType.Mind, 1 },
            { AttributeType.Endurance, 1 },
            { AttributeType.Strength, 1 },
            { AttributeType.Dexterity, 1 },
            { AttributeType.Intelligence, 1 },
            { AttributeType.Faith, 1 },
            { AttributeType.Charisma, 1 }
        };

        // Mob Special Stats
        private Dictionary<SpecialStatType, float> m_SpecialStatTypeToAmountFactor = new Dictionary<SpecialStatType, float>() {
            { SpecialStatType.SpeedMultiplier, 1.0f },
            { SpecialStatType.CritChance, 0.1f },
            { SpecialStatType.CritDamageMultiplier, 2.0f },
            { SpecialStatType.BlockPhysicalAmount, 0.5f },
            // { SpecialStatType.MagicCritChance, 1.0f },
            // { SpecialStatType.MagicCritDamage, 1.0f },
            // { SpecialStatType.MagicBlockAmount, 1.0f },
            // { SpecialStatType.HealthRegen, 1.0f },
            // { SpecialStatType.ManaRegen, 1.0f },
            // { SpecialStatType.StaminaRegen, 1.0f },
            // { SpecialStatType.EquipWeightModifier, 1.0f },
            // { SpecialStatType.Luck, 1.0f }
        };

        // Mob Resistances
        private Dictionary<DamageType, float> m_DamageTypeToResistanceAmountFactor = new Dictionary<DamageType, float>() {
            { DamageType.Physical, 1.0f },
            { DamageType.Fire, 1.0f },
            { DamageType.Ice, 1.0f },
            { DamageType.Lightning, 1.0f }
        };

        // Getters
        public float GetLevelEffectFactor() { return LEVEL_EFFECT_FACTOR; }
        public Dictionary<BaseStatType, float> GetMaxBaseStatValues() { return m_BaseStatTypeToMaxValue; }
        public Dictionary<BaseStatType, float> GetCurrentBaseStatValues() { return m_BaseStatTypeToCurrentValue; }
        public Dictionary<AttributeType, int> GetCurrentAttributeLevels() { return m_AttributeTypeToCurrentLevel; }
        public Dictionary<SpecialStatType, float> GetSpecialStatAmountFactors() { return m_SpecialStatTypeToAmountFactor; }
        public Dictionary<DamageType, float> GetResistanceAmountFactors() { return m_DamageTypeToResistanceAmountFactor; }

        // Setters
        public void SetMaxBaseStatValue(BaseStatType baseStatType, float amount) { m_BaseStatTypeToMaxValue[baseStatType] = amount; }
        public void SetCurrentBaseStatValue(BaseStatType baseStatType, float amount) { m_BaseStatTypeToCurrentValue[baseStatType] = amount; }
        public void SetCurrentAttributeLevel(AttributeType attributeType, int level) { m_AttributeTypeToCurrentLevel[attributeType] = level; }
        public void SetSpecialStatAmountFactor(SpecialStatType specialStatType, float amount) { m_SpecialStatTypeToAmountFactor[specialStatType] = amount; }
        public void SetResistanceAmountFactor(DamageType damageType, float amount) { m_DamageTypeToResistanceAmountFactor[damageType] = amount; }
    }

    public class WeaponStats
    {
        public enum WeaponType
        {
            Melee,
            HitScan,
            Projectile
        }

        // m_DamageTypeToDamageAmount 
        // NOTE: DamageTable is passed by reference to the Attacking Mob so that they can apply their own damage modifiers/effects,
        // then the table is once again passed by reference to the Attacked Mob so that they can apply their own resistance modifiers/effects
        private Dictionary<DamageType, float> m_DamageTable = new Dictionary<DamageType, float>() {
            { DamageType.Physical, 0.0f },
            { DamageType.Fire, 0.0f },
            { DamageType.Ice, 0.0f },
            { DamageType.Lightning, 0.0f }
        };

        private int m_WeaponLevel = 1;
        private WeaponType m_WeaponType;

        // Getters
        public int GetWeaponLevel() { return m_WeaponLevel; }
        public WeaponType GetWeaponType() { return m_WeaponType; }
        public ref Dictionary<DamageType, float> GetDamageTable() { return ref m_DamageTable; }

        // Setters
        public void SetWeaponLevel(int level) { m_WeaponLevel = level; }
        public void SetWeaponType(WeaponType weaponType) { m_WeaponType = weaponType; }
        public void SetDamageAmount(DamageType damageType, float damageAmount) { m_DamageTable[damageType] = damageAmount; }
    }
}
