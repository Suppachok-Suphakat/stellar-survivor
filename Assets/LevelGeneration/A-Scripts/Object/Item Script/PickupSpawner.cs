using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ItemDrop
    {
        public GameObject itemPrefab;
        [Range(0, 100)]
        public float dropChance; // Drop chance percentage (0 to 100)
    }

    [SerializeField] private ItemDrop[] itemDrops;

    public void DropItems()
    {
        foreach (ItemDrop itemDrop in itemDrops)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= itemDrop.dropChance)
            {
                Instantiate(itemDrop.itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}