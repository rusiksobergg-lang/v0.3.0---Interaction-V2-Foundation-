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

    [Header("Condition")]
    public bool useCondition = true;

    [Min(1)]
    public int maxCondition = 100;

    public bool canBeRepaired = true;

    [Header("UI")]
    public Sprite icon;
}