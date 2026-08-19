using UnityEngine;

public class WorldItem : Interactable
{
    [Header("Runtime Instance")]
    [SerializeField]
    private ItemInstance itemInstance;

    private void Awake()
    {
        if (itemData != null)
        {
            itemInstance = new ItemInstance(itemData);
        }
    }

    public ItemInstance GetItemInstance()
    {
        return itemInstance;
    }

    public void Initialize(ItemInstance existingInstance)
    {
        itemInstance = existingInstance;
    }
}