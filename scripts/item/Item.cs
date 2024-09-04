using Godot;

public partial class Item : Node3D
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    // NOTE: Only for equippable items
    public enum ItemSlot
    {
        Head,
        Chest,
        Legs,
        Feet,
        Wrist,
        MainHand,
        OffHand,
        Ring1,
        Ring2,
        Neck,
        Back,
        Consumable1,
        Consumable2,
        Consumable3,
    }

    [Export]
    public StringName m_ItemName { get; private set; } = "Item";
    [Export]
    public int m_MaxStackSize { get; private set; } = 1;
    [Export]
    public Mob m_Owner { get; private set; } // The Mob that owns this item - if null, assume it's in the world
    [Export]
    public Rarity m_Rarity { get; private set; }
    [Export]
    public ItemSlot m_ItemSlotType { get; private set; }
    [Export]
    public bool m_IsEquippable { get; private set; } = false;
    [Export]
    public bool m_IsPickupable { get; private set; } = false;
    [Export]
    public bool m_IsDroppable { get; private set; } = false;
    [Export]
    public bool m_IsConsumable { get; private set; } = false;
    // TODO: public Texture m_GuiIcon;


    // Called when a body enters the Area3D - Body is any PhysicsBody3D (CharacterBody3D, RigidBody3D, etc...)
    public void _OnPickupArea3DBodyEntered(Node3D body)
    {
        if (body is Mob mob)
        {
            GD.Print("Picked up Dragon Slayer");
            mob.PickUpItem(this);
        }
    }

    // -------------------------------------------
    // Setters
    // -------------------------------------------
    public void SetItemName(StringName itemName) { m_ItemName = itemName; }
    public void SetMaxStackSize(int maxStackSize) { m_MaxStackSize = maxStackSize; }
    public void SetOwner(Mob owner) { m_Owner = owner; }
    public void SetRarity(Rarity rarity) { m_Rarity = rarity; }
    public void SetItemSlotType(ItemSlot itemSlotType) { m_ItemSlotType = itemSlotType; }
    public void SetIsEquippable(bool isEquippable) { m_IsEquippable = isEquippable; }
    public void SetIsPickupable(bool isPickupable) { m_IsPickupable = isPickupable; }
    public void SetIsDroppable(bool isDroppable) { m_IsDroppable = isDroppable; }
    public void SetIsConsumable(bool isConsumable) { m_IsConsumable = isConsumable; }
}
