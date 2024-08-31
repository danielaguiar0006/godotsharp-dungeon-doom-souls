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
    public StringName m_ItemName = "Item";
    [Export]
    public Mob m_Owner; // The Mob that owns this item - if null, assume it's in the world
    [Export]
    public Rarity m_Rarity;
    [Export]
    public ItemSlot m_ItemSlotType;
    [Export]
    public int m_MaxStackSize = 1;
    [Export]
    public bool m_IsEquippable = false;
    [Export]
    public bool m_IsPickupable = false;
    [Export]
    public bool m_IsDroppable = false;
    [Export]
    public bool m_IsStackable = false;
    [Export]
    public bool m_IsConsumable = false;
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
}
