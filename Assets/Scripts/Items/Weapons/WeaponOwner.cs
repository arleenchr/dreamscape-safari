using BondomanShooter.Entities;
using BondomanShooter.Structs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BondomanShooter.Items.Weapons {
    public class WeaponOwner : MonoBehaviour, IHasDamageModifier {
        [Header("References")]
        [SerializeField] private Animator ownerAnimator;

        [Header("Weapons")]
        [SerializeField] private Transform holder;
        [SerializeField] private WeaponEntityInstance[] initialWeapons;

        public IEnumerable<BaseWeapon> Weapons => weapons;
        public BaseWeapon SelectedWeapon => 0 <= selectedWeaponIndex && selectedWeaponIndex < weapons.Count ? weapons[selectedWeaponIndex] : null;
        public int SelectedWeaponIndex => selectedWeaponIndex;
        public bool[] EnabledWeapons { get; private set; }
        public bool IsOneHitKill { get; set; }
        //public string Difficulty => SettingsController.Instance.Difficulty;

        public Animator OwnerAnimator => ownerAnimator;
        public ModifierStack<float> DamageModifier { get; } = new ModifierStack<float>(1f);
        //public float DamageMultiplier {
        //    get => IsOneHitKill ? float.PositiveInfinity :
        //        (CompareTag("Player") ? 1f :
        //            (Difficulty == "Medium" ? 0.5f : (Difficulty == "Hard" ? 1.5f : 1f))
        //        ) * damageMultiplier;
        //    set {
        //        if(value > 2.5f) {
        //            damageMultiplier = 2.5f;
        //        } else {
        //            damageMultiplier = value;
        //        }
        //    }
        //}
        public int NumDamageBoosts {
            get => numDamageBoosts;
            set => numDamageBoosts = Mathf.Min(25, value);
        }
        public float DamageMultFromPets { get; set; } = 1f;
        public bool IsParrying => SelectedWeapon is ParryWeapon parryWeapon && parryWeapon.Parrying;

        private List<BaseWeapon> weapons;
        private RuntimeAnimatorController ownerOriginalAnimatorController;
        private int selectedWeaponIndex = -1;
        private int numDamageBoosts = 0;
        //private float damageMultiplier = 1f;

        private void Awake() {
            DamageModifier.Add(100, new DamageDifficultyMultiplier());
            DamageModifier.Add(10, new DamageBoostMultiplier(this));

            ownerOriginalAnimatorController = ownerAnimator.runtimeAnimatorController;

            weapons = new List<BaseWeapon>();
            EnabledWeapons = new bool[initialWeapons.Length];

            int initialSelectedIndex = -1;
            for(int i = 0; i < initialWeapons.Length; i++) {
                WeaponEntityInstance weaponEntity = initialWeapons[i];

                // Instantiate weapon game object
                GameObject instance = Instantiate(weaponEntity.Weapon.gameObject, holder);
                instance.transform.SetLocalPositionAndRotation(weaponEntity.OffsetPosition, weaponEntity.OffsetRotation);
                instance.transform.localScale = weaponEntity.OffsetScale;

                // Set weapon attributes
                BaseWeapon weapon = instance.GetComponent<BaseWeapon>();
                weapon.Owner = this;
                weapon.ModelHolder.gameObject.SetActive(false);

                // Push weapon into the owner list
                weapons.Add(weapon);
                EnabledWeapons[i] = weaponEntity.Enabled;

                // Set the first enabled weapon as the initial selected weapon
                if(initialSelectedIndex < 0 && weaponEntity.Enabled) initialSelectedIndex = i;
            }

            // Enable initially selected weapon
            Select(initialSelectedIndex);
        }

        //private void Start()
        //{
        //    Debug.Log(Difficulty);
        //}

        public void PrimaryAction(bool performing) {
            BaseWeapon selected = SelectedWeapon;
            if(selected != null) {
                selected.PrimaryAction(performing);
            } else {
                Debug.LogWarning($"No weapon selected for {gameObject.name}");
            }
        }

        public BaseWeapon Select(int weaponIndex) {
            // Throw if weapon index is out of range
            if(weaponIndex < 0 || weaponIndex >= weapons.Count)
                throw new IndexOutOfRangeException($"Index {weaponIndex} out of bounds for weapon list of size {weapons.Count}");

            if(selectedWeaponIndex != weaponIndex) {
                // Stop action for and disable currently selected weapon (if any)
                if(selectedWeaponIndex >= 0) {
                    BaseWeapon current = SelectedWeapon;
                    current.PrimaryAction(false);
                    current.ModelHolder.gameObject.SetActive(false);
                }

                // Enable newly selected weapon
                selectedWeaponIndex = weaponIndex;
                SelectedWeapon.ModelHolder.gameObject.SetActive(true);

                RuntimeAnimatorController overrideController = initialWeapons[SelectedWeaponIndex].OwnerOverrideAnimatorController;
                if(overrideController != null) {
                    // Override animator if there is one
                    ownerAnimator.runtimeAnimatorController = overrideController;
                } else {
                    // Reset the owner animator to the original one
                    ownerAnimator.runtimeAnimatorController = ownerOriginalAnimatorController;
                }
            }

            return SelectedWeapon;
        }
        
        public void OneHitKill()
        {
            IsOneHitKill = !IsOneHitKill;
        }
    }

    [Serializable]
    public class WeaponEntityInstance {
        [SerializeField] private BaseWeapon weapon;
        [SerializeField] private bool enabled;

        [Header("Transform Offsets")]
        [SerializeField] private Vector3 offsetPosition = Vector3.zero;
        [SerializeField] private Quaternion offsetRotation = Quaternion.identity;
        [SerializeField] private Vector3 offsetScale = Vector3.one;

        [Header("Overrides")]
        [SerializeField] private RuntimeAnimatorController overrideAnimatorController;

        public BaseWeapon Weapon => weapon;
        public bool Enabled => enabled;
        public Vector3 OffsetPosition => offsetPosition;
        public Quaternion OffsetRotation => offsetRotation;
        public Vector3 OffsetScale => offsetScale;

        public RuntimeAnimatorController OwnerOverrideAnimatorController => overrideAnimatorController;
    }

    public class DamageBoostMultiplier : ModifierStack<float>.IModifierItem {
        private readonly WeaponOwner owner;

        public ModifierStack<float>.Modifier Mod => (float damage) => damage * (1f + owner.NumDamageBoosts * 0.1f);
        public bool ShouldRemove => false;

        public DamageBoostMultiplier(WeaponOwner owner) {
            this.owner = owner;
        }

        public void OnRemoved() { }
    }

    public class DamageDifficultyMultiplier : ModifierStack<float>.IModifierItem {
        public ModifierStack<float>.Modifier Mod => (float damage) => damage * MultiplierFromDifficulty();
        public bool ShouldRemove => false;

        private float MultiplierFromDifficulty() {
            switch(SettingsController.Instance.Difficulty) {
                case "Easy": return 0.5f;
                case "Medium": return 1f;
                case "Hard": return 1.5f;
                default: throw new System.Exception($"Unknown difficulty settings '{SettingsController.Instance.Difficulty}'");
            }
        }

        public void OnRemoved() { }
    }
}
