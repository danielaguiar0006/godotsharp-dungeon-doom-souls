using System.Collections.Generic;

namespace Game.DamageSystem
{
    // HACK: using DamageTable = Dictionary<DamageType, float>;

    public enum DamageType
    {
        Physical,
        // Blunt,
        // Pierce,
        // Slash,
        Fire,
        Ice,
        Lightning
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


    // NOTE: We are instead using a Dictionary<DamageType, float> for the damage table for simplicity
    //
    // public class DamageTable
    // {
    //     private Dictionary<DamageType, float> m_DamageTable = new Dictionary<DamageType, float>();
    //
    //     public DamageTable()
    //     {
    //         foreach (DamageType damageType in (DamageType[])System.Enum.GetValues(typeof(DamageType)))
    //         {
    //             m_DamageTable.Add(damageType, 0.0f);
    //         }
    //     }
    //
    //     public void SetDamageTable(Dictionary<DamageType, float> damageTable)
    //     {
    //         m_DamageTable = damageTable;
    //     }
    //
    //     public void SetDamage(DamageType damageType, float damageAmount)
    //     {
    //         m_DamageTable[damageType] = damageAmount;
    //     }
    //
    //     public float GetDamage(DamageType damageType)
    //     {
    //         return m_DamageTable[damageType];
    //     }
    //
    //     public Dictionary<DamageType, float> GetDamageTable()
    //     {
    //         return m_DamageTable;
    //     }
    // }
}
