using System.Collections.Generic;

namespace Game.DamageSystem
{
    // HACK: using DamageTable = Dictionary<DamageType, float>;

    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning
        // Blunt,
        // Pierce,
        // Slash,
    }

    public struct Damage
    {
        public DamageType m_DamageType;
        public Dictionary<DamageType, float> m_DamageTable;

        public Damage(DamageType damageType, Dictionary<DamageType, float> damageTable)
        {
            m_DamageType = damageType;
            m_DamageTable = damageTable;
        }

        public Damage()
        {
            m_DamageTable = new Dictionary<DamageType, float>() {
                { DamageType.Physical, 0.0f },
                { DamageType.Fire, 0.0f },
                { DamageType.Ice, 0.0f },
                { DamageType.Lightning, 0.0f }
            };
        }
    }
}
