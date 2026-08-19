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

    [Header("Stack")]
    public int quantity = 1;

    public bool IsRuined =>
        itemData != null &&
        itemData.useCondition &&
        currentCondition <= 0;

    public ItemInstance(ItemData data)
    {
        itemData = data;
        uniqueID = Guid.NewGuid().ToString();
        quantity = 1;

        if (data != null)
        {
            if (data.useCondition)
            {
                int min = Mathf.RoundToInt(data.maxCondition * (data.minSpawnCondition / 100f));
                int max = Mathf.RoundToInt(data.maxCondition * (data.maxSpawnCondition / 100f));

                currentCondition = UnityEngine.Random.Range(min, max + 1);
            }
            else
            {
                currentCondition = 0;
            }
        }
    }

    public void Damage(int amount)
    {
        if (itemData == null || !itemData.useCondition)
            return;

        currentCondition = Mathf.Max(0, currentCondition - amount);
    }

    public void Repair(int amount)
    {
        if (itemData == null || !itemData.useCondition || !itemData.canBeRepaired)
            return;

        currentCondition = Mathf.Min(itemData.maxCondition, currentCondition + amount);
    }

    public void SetCondition(int value)
    {
        if (itemData == null || !itemData.useCondition)
            return;

        currentCondition = Mathf.Clamp(value, 0, itemData.maxCondition);
    }
}