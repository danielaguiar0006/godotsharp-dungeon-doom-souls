using Godot;
using System.Collections.Generic;
using static InputActions;
using Game.StatsManager;
using Game.DamageSystem;
using Game.StateMachines;

public partial class Mob : CharacterBody3D
{
    public enum MobType
    {
        Monster,
        NPC,
        Boss,
        Player
    }

    [Export]
    public StringName m_Name = "Mob";
    [Export]
    public MobType m_MobType { get; private set; }
    [Export]
    public bool m_IsHostile { get; private set; } = true; // TODO: think about what you want to have private set and not...
    [Export]
    public float m_MovementSpeed { get; private set; } = 5.0f;
    [Export]
    public MobState m_CurrentMobState { get; protected set; } = null;
    [Export]
    public Vector3 m_MovementDirection { get; private set; } = Vector3.Zero; // Where the mob is moving towards
    [Export]
    public float m_JumpVelocity = 4.0f;
    [Export]
    public Item[] m_InventoryItems { get; private set; }
    // TODO: Make an inventory class & object which handles storing items, equiping, and unequiping items (Basically an InvenotryManager):

    // Get the gravity from the project settings to be synced with RigidBody nodes
    public float m_Gravity { get; private set; } = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public Dictionary<Item.ItemSlot, Item> m_EquipedItems { get; private set; } = new Dictionary<Item.ItemSlot, Item>();
    public MobStats m_MobStats { get; private set; } = new MobStats();


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
            movementSpeedFactor *= m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.MovementSpeedFactor];
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
            movementSpeedFactor *= m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.MovementSpeedFactor];
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

    // TODO:
    public void TakeDamage(ref Damage damage)
    {
        float currentHealth = m_MobStats.m_BaseStatTypeToCurrentValue[BaseStatType.Health];

        // Apply damage resistance
        var resistanceFactors = m_MobStats.m_DamageTypeToResistanceAmountFactor;
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
        m_MobStats.SetCurrentBaseStatValue(BaseStatType.Health, currentHealth);
        GD.Print("Mob's Health: " + m_MobStats.m_BaseStatTypeToCurrentValue[BaseStatType.Health]);

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    // TODO:
    // The target Mob is figured out in the AttackState
    // NOTE: We pass a ref to damageTable so that the weapon can pass different damage values to the mob depending on the type of attack (light, heavy, special, etc...)
    public void Attack(Damage damage, Mob target)
    {
        // if (!m_IsAlive)
        // {
        //     return;
        // }
        //
        // if (target == null || !target.m_IsAlive)
        // {
        //     return;
        // }

        // Apply Attribute effects
        var attributeLevels = m_MobStats.m_AttributeTypeToCurrentLevel;
        foreach (var damageTypeToDamageAmount in damage.m_DamageTable)
        {
            if (damageTypeToDamageAmount.Key == DamageType.Physical)
            {
                damage.m_DamageTable[DamageType.Physical] = damageTypeToDamageAmount.Value * (attributeLevels[AttributeType.Strength] * m_MobStats.m_LevelEffectFactor);
            }
        }

        // Apply special effects
        var specialStats = m_MobStats.m_SpecialStatTypeToAmountFactor;
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

    // TODO:
    public void Die()
    {
        GD.Print(this.m_Name + " has died");
        GameManager.Instance.DespawnMob(this);
    }

    // TODO:
    public void PickUpItem(Item item)
    {
        if (item == null || !item.m_IsPickupable || item.m_Owner != null)
        {
            return;
        }

        item.SetOwner(this);

        if (item.m_IsEquippable)
        {
            EquipItem(item);
        }
        else
        {
            AddItemToInventory(item);
        }
    }

    // TODO:
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
        this.GetNode<Node3D>("UntieFromPhysicsMovement").CallDeferred("add_child", item);
    }

    // TODO:
    public void AddItemToInventory(Item item)
    {
        if (item == null || !item.m_IsPickupable)
        {
            return;
        }
        // TODO: Add item to inventory
    }

    // Change the mob's state
    public void TransitionToState(MobState newState)
    {
        if (newState != null)
        {
            if (m_CurrentMobState != null) { m_CurrentMobState.OnExitState(this); } // Exit current state 
            m_CurrentMobState = newState;        // Set the new state
            MobState nextState = m_CurrentMobState.OnEnterState(this); // Enter the new state

            // If the OnEnterState of the new state returns another state, transition again
            if (nextState != null)
            {
                TransitionToState(nextState);
            }
        }
    }

    // -------------------------------------------
    // Setters
    // -------------------------------------------
    public void SetMobType(MobType mobType) { m_MobType = mobType; }
    public void SetIsHostile(bool isHostile) { m_IsHostile = isHostile; }
    public void SetMovementSpeed(float movementSpeed) { m_MovementSpeed = movementSpeed; }
    public void SetMovementDirection(Vector3 movementDirection) { m_MovementDirection = movementDirection; }
    // NOTE: SetGravity() is not needed as it should be synced with the project settings
    public void SetInventoryItems(Item[] inventoryItems) { m_InventoryItems = inventoryItems; }
    public void SetEquipedItems(Dictionary<Item.ItemSlot, Item> equipedItems) { m_EquipedItems = equipedItems; }
    public void SetStats(MobStats stats) { m_MobStats = stats; }
}
