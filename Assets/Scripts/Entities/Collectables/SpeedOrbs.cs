using BondomanShooter.Entities.Player;
using BondomanShooter.Structs;
using UnityEngine;

namespace BondomanShooter.Entities.Collectibles {
    public class SpeedOrbs : Orb {
        public float speedBonus;
        public float duration;

        public override void ApplyEffect(PlayerController player) {
            base.ApplyEffect(player);

            if(OrbSpeedBoost.CurrentActive != null) {
                OrbSpeedBoost.CurrentActive.Refresh();
            } else {
                if(player.TryGetComponent(out IHasSpeedModifier speedMod)) {
                    speedMod.SpeedModifier.Add(0, new OrbSpeedBoost(speedBonus, duration));
                }
            }
        }
    }

    public class OrbSpeedBoost : ModifierStack<float>.IModifierItem {
        public static OrbSpeedBoost CurrentActive { get; private set; }

        public ModifierStack<float>.Modifier Mod => (float speed) => speed * (1f + speedBonus);
        public bool ShouldRemove => Time.time > beginTime + duration;

        private readonly float speedBonus, duration;
        private float beginTime;

        public OrbSpeedBoost(float speedBonus, float duration) {
            if(CurrentActive != null) Debug.LogError("A speed boost for player already exists!");
            CurrentActive = this;

            this.speedBonus = speedBonus;
            this.duration = duration;
            beginTime = Time.time;
        }

        public void Refresh() {
            beginTime = Time.time;
        }

        public void OnRemoved() {
            CurrentActive = null;
        }
    }
}
