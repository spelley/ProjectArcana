public struct InventoryQuantity
{
    public ItemData itemData;
    public int quantity;
    public int numEquipped;

    public InventoryQuantity(ItemData itemData, int quantity, int numEquipped)
    {
        this.itemData = itemData;
        this.quantity = quantity;
        this.numEquipped = numEquipped;
    }
}