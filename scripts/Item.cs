using Godot;

// NOTE: This should basically only contain data/state information like a struct,
// It will be the child of the Item class which will contain all the functionality
// and for other systems like an inventory system to enforce the m_IsStackable for example
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

    protected Mob m_Owner; // The Mob that owns this item - if null, assume it's in the world
    protected Rarity m_Rarity;
    protected StringName m_ItemName = "Item";
    protected int m_MaxStackSize = 1;
    protected bool m_IsEquippable = false;
    protected bool m_IsPickupable = false;
    protected bool m_IsDroppable = false;
    protected bool m_IsStackable = false;
    protected bool m_IsConsumable = false;
    // TODO: protected Texture m_GuiIcon;
}
