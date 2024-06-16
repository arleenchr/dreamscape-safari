using BondomanShooter.Entities;
using BondomanShooter.Items.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace BondomanShooter.Game {
    [RequireComponent(typeof(Collider))]
    public class SpawnOnDamaged : MonoBehaviour, IHealth {
        [Header("Spawner")]
        [SerializeField] private GameObject spawnObject;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private string damageSourceTag;
        [SerializeField] private UnityEvent<GameObject> onMobSpawn;

        public int Health => 1;
        public int MaxHealth => 1;

        public DamageResult ApplyDamage(WeaponOwner source, int damage, Vector3 location, bool parriable = true) {
            if(source != null && source.gameObject.CompareTag(damageSourceTag)) {
                GameObject obj = Instantiate(spawnObject, spawnPosition.position, spawnPosition.rotation);
                onMobSpawn.Invoke(obj);
            }

            return DamageResult.None;
        }
    }
}
