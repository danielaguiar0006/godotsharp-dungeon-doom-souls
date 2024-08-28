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

    protected Rarity m_Rarity;
    protected StringName m_ItemName = "Item";
    protected bool m_IsEquippable = false;
    protected bool m_IsPickupable = false;
    protected bool m_IsDroppable = false;
    protected bool m_IsStackable = false;
    protected bool m_IsConsumable = false;
    // TODO: protected Texture m_GuiIcon;
}
