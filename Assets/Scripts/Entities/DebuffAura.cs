using BondomanShooter.Structs;
using System.Collections.Generic;
using UnityEngine;

namespace BondomanShooter.Entities {
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class DebuffAura : MonoBehaviour {
        [SerializeField] private string targetTag;
        [SerializeField] private int damagePerSecond;
        [SerializeField] private float speedMalus;
        [SerializeField] private float damageMalus;

        private float lastDamageTimestamp;

        private Dictionary<GameObject, (DebuffAuraFloatMultiplier speedMultiplier, DebuffAuraFloatMultiplier damageMultiplier)> debuffedObjects;

        private void Awake() {
            debuffedObjects = new Dictionary<GameObject, (DebuffAuraFloatMultiplier speedMultiplier, DebuffAuraFloatMultiplier damageMultiplier)>();
        }

        private void Update() {
            // Apply per-second damage to targets
            if(debuffedObjects.Count > 0 && Time.time > lastDamageTimestamp + 1f) {
                foreach(GameObject obj in debuffedObjects.Keys) {
                    IHealth health = obj.GetComponentInParent<IHealth>();
                    health?.ApplyDamage(null, damagePerSecond, transform.position, false);
                }
                lastDamageTimestamp = Time.time;
            }
        }

        private void ApplyTo(GameObject obj) {
            if(!debuffedObjects.ContainsKey(obj)) {
                DebuffAuraFloatMultiplier speedMult = null, damageMult = null;

                // Add multipliers to the object
                if(speedMalus != 0f && obj.TryGetComponent(out IHasSpeedModifier speedMod)) {
                    speedMult = new DebuffAuraFloatMultiplier(1f - speedMalus);
                    speedMod.SpeedModifier.Add(1, speedMult);
                }

                if(damageMalus != 0f && obj.TryGetComponent(out IHasDamageModifier damageMod)) {
                    damageMult = new DebuffAuraFloatMultiplier(1f - damageMalus);
                    damageMod.DamageModifier.Add(1, damageMult);
                }

                // Keep track of multipliers
                debuffedObjects.Add(obj, (speedMult, damageMult));
            }
        }

        private void UnapplyTo(GameObject obj) {
            if(debuffedObjects.ContainsKey(obj)) {
                // Mark multipliers for removal
                var (speedMult, damageMult) = debuffedObjects[obj];

                if(speedMult != null) speedMult.ShouldRemove = true;
                if(damageMult != null) damageMult.ShouldRemove = true;

                // Remove object from multiplier tracker
                debuffedObjects.Remove(obj);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if(other.CompareTag(targetTag)) ApplyTo(other.gameObject);
        }

        private void OnTriggerExit(Collider other) {
            if(other.CompareTag(targetTag)) UnapplyTo(other.gameObject);
        }
    }

    public class DebuffAuraFloatMultiplier : ModifierStack<float>.IModifierItem {
        public ModifierStack<float>.Modifier Mod => (float attr) => attr * multiplier;
        public bool ShouldRemove { get; set; }

        private readonly float multiplier;

        public DebuffAuraFloatMultiplier(float multiplier) {
            this.multiplier = multiplier;
        }

        public void OnRemoved() { }
    }
}
