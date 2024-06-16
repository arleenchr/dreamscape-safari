using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour {
    // Reference: https://www.youtube.com/watch?v=KjvvRmG7PBM

    public float dropForce = 30f;
    public List<Loot> lootList = new();

    List<Loot> GetDroppedItems() {
        List<Loot> possibleItems = new();

        foreach(Loot item in lootList) {
            if(Random.value <= item.dropChance) {
                possibleItems.Add(item);
            }
        }

        if(possibleItems.Count > 0) {
            return (possibleItems);
        }

        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition) {
        List<Loot> droppedItem = GetDroppedItems();
        if(droppedItem != null) {
            foreach(Loot item in droppedItem) {
                GameObject lootGameObject = Instantiate(item.lootPrefab, spawnPosition, Quaternion.identity);

                Vector3 dropDirection = new(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
                if(lootGameObject.TryGetComponent(out Rigidbody rb)) {
                    rb.AddForce(dropDirection * dropForce, ForceMode.Impulse);
                }
            }
        }
    }
}
