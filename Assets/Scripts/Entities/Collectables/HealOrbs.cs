using BondomanShooter.Entities.Player;
using UnityEngine;

namespace BondomanShooter.Entities.Collectibles {
    public class HealOrbs : Orb {
        [SerializeField] private int healAmount;

        public override void ApplyEffect(PlayerController player) {
            base.ApplyEffect(player);
            player.ApplyDamage(null, -healAmount, transform.position, false);
        }
    }
}
