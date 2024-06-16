using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BondomanShooter/Loot")]
public class Loot : ScriptableObject
{
    public GameObject lootPrefab;
    [Range(0f, 1f)] public float dropChance;
}
