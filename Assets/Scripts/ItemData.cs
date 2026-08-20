using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemID;
    public string displayName;

    [Header("Category")]
    public ItemCategory category;

    [Header("Description")]
    [TextArea(2, 5)]
    public string description;

    [Header("Inventory")]
    public float weight = 1f;

    [Min(1)]
    public int inventoryWidth = 1;

    [Min(1)]
    public int inventoryHeight = 1;

    [Header("Condition")]
    public bool useCondition = true;

    [Header("Spawn Condition")]
    [Range(0, 100)]
    public int minSpawnCondition = 40;

    [Range(0, 100)]
    public int maxSpawnCondition = 100;

    [Min(1)]
    public int maxCondition = 100;

    public bool canBeRepaired = true;

    [Header("UI")]
    public Sprite icon;
}