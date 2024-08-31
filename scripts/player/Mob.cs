using Godot;
using static InputActions;
using Game.StatsAndAttributes;
using Game.DamageSystem;
using System.Collections.Generic;

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
    // TODO: Make an inventory class & object which handles storing items, equiping, and unequiping items (Basically an InvenotryManager):
    private Item[] m_InventoryItems;
    public Dictionary<Item.ItemSlot, Item> m_EquipedItems = new Dictionary<Item.ItemSlot, Item>();


    public float m_MovementSpeed = 5.0f;
    // Where the mob is moving towards
    public Vector3 m_MovementDirection = Vector3.Zero;

    // Get the gravity from the project settings to be synced with RigidBody nodes
    public float m_Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


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
        GD.Print("Mob's Health: " + m_Stats.GetCurrentBaseStatValues()[BaseStatType.Health]);

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

    // moveSpeedFactor can be used for slower or faster movement (walking, running, etc...)
    public void ApplyMobMovement(ref Vector3 velocity)
    {
        // Move the mob
        this.Velocity = velocity;
        this.MoveAndSlide();
    }

    // Helper function to apply gravity to a vector in different states
    // This does not apply gravity directly to the mob's velocity, but instead to a target vector
    public void ApplyGravityToVector(ref Vector3 velocity, double delta)
    {
        // Add the gravity
        if (!this.IsOnFloor())
            velocity.Y -= this.m_Gravity * (float)delta;
    }

    // This does not apply input movement directly to the mob's velocity, but instead to a target vector
    public void ApplyMovementInputToVector(ref Vector3 velocity, float movementSpeedFactor = 1.0f, bool applyStatMovementSpeedFactor = true)
    {
        if (applyStatMovementSpeedFactor)
        {
            movementSpeedFactor *= m_Stats.GetSpecialStatAmountFactors()[SpecialStatType.MovementSpeedFactor];
        }

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector(s_MoveLeft, s_MoveRight, s_MoveForward, s_MoveBackward);
        this.m_MovementDirection = (this.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (this.m_MovementDirection != Vector3.Zero)
        {
            velocity.X = m_MovementDirection.X * m_MovementSpeed * movementSpeedFactor;
            velocity.Z = this.m_MovementDirection.Z * this.m_MovementSpeed * movementSpeedFactor;
        }
        else  // If the mob is not moving, decelerate
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, this.m_MovementSpeed * movementSpeedFactor);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, this.m_MovementSpeed * movementSpeedFactor);
        }
    }

    // This does not apply movement direction directly to the mob's velocity, but instead to a target vector
    // This also does not apply the mob's stat: MovementSpeedFactor on top of the local movementSpeedFactor,
    // if you do want to apply the mob's MovementSpeedFactor, manually apply it or use ApplyMovementInputToVector instead.
    public void ApplyMovementDirectionToVector(ref Vector3 velocity, Vector3 wishDirection, float movementSpeedFactor = 1.0f, bool applyStatMovementSpeedFactor = true)
    {
        if (applyStatMovementSpeedFactor)
        {
            movementSpeedFactor *= m_Stats.GetSpecialStatAmountFactors()[SpecialStatType.MovementSpeedFactor];
        }

        if (wishDirection != Vector3.Zero)
        {
            velocity.X = wishDirection.X * this.m_MovementSpeed * movementSpeedFactor;
            velocity.Z = wishDirection.Z * this.m_MovementSpeed * movementSpeedFactor;
        }
        else
        {
            velocity.X = Mathf.MoveToward(this.Velocity.X, 0, this.m_MovementSpeed * movementSpeedFactor);
            velocity.Z = Mathf.MoveToward(this.Velocity.Z, 0, this.m_MovementSpeed * movementSpeedFactor);
        }
    }

    public void PickUpItem(Item item)
    {
        if (item == null || !item.m_IsPickupable || item.m_Owner != null)
        {
            return;
        }

        item.m_Owner = this;

        if (item.m_IsEquippable)
        {
            EquipItem(item);
        }
        else
        {
            AddItemToInventory(item);
        }
    }

    public void EquipItem(Item item)
    {
        if (item == null || !item.m_IsEquippable)
        {
            return;
        }

        // TODO:
        // if (m_EquipedItems.ContainsKey(item.m_ItemSlotType))
        // {
        //     // TODO: Unequip the item in the slot
        //     //UnequipItem(m_EquipedItems[item.m_ItemSlotType]);
        // }
        // else
        // {
        // }

        GD.Print("Equipping " + item.m_ItemName);
        m_EquipedItems.Add(item.m_ItemSlotType, item);
        item.Transform = this.GetNode<Marker3D>("EquipmentTargetTransforms/TargetMainHand").Transform;

        // NOTE: Needs to be called deferred to avoid potential physics timing related issues
        item.GetParent().CallDeferred("remove_child", item);
        this.CallDeferred("add_child", item);
    }

    public void AddItemToInventory(Item item)
    {
        if (item == null || !item.m_IsPickupable)
        {
            return;
        }
        // TODO: Add item to inventory
    }
}
