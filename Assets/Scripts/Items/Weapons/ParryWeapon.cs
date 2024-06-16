using UnityEngine;

namespace BondomanShooter.Items.Weapons {
    public abstract class ParryWeapon : BaseWeapon {
        [Header("Parry Weapon")]
        [SerializeField] protected bool canParry;
        [SerializeField] protected float parryArc;

        public bool Parrying { get; protected set; }
        public float ParryArc => parryArc;
    }
}
