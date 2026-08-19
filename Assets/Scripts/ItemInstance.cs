using System;
using UnityEngine;

[Serializable]
public class ItemInstance
{
    [Header("Template")]
    public ItemData itemData;

    [Header("Identity")]
    public string uniqueID;

    [Header("Condition")]
    public int currentCondition;

    public ItemInstance(ItemData data)
    {
        itemData = data;
        uniqueID = Guid.NewGuid().ToString();

        currentCondition = data != null ? data.maxCondition : 100;
    }
}