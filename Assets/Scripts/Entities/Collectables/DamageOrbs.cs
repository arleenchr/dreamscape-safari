using BondomanShooter.Entities.Player;

namespace BondomanShooter.Entities.Collectibles {
    public class DamageOrbs : Orb {

        public override void ApplyEffect(PlayerController player) {
            base.ApplyEffect(player);
            player.WeaponOwner.NumDamageBoosts++;
        }
    }
}

