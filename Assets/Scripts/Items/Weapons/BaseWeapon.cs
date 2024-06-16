using UnityEngine;

namespace BondomanShooter.Items.Weapons {
    public abstract class BaseWeapon : MonoBehaviour, IDisplayable {
        [Header("Displayable")]
        [SerializeField] protected string weaponName;
        [SerializeField] protected string weaponDescription;

        [Header("Rendering")]
        [SerializeField] protected Transform modelHolder;

        public WeaponOwner Owner { get; set; }
        public Transform ModelHolder => modelHolder;

        public abstract int Damage { get; }

        public string Name => weaponName;
        public string Description => weaponDescription;

        public abstract void PrimaryAction(bool performing);
    }
}
