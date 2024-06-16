using BondomanShooter.Entities.Mobs;
using UnityEngine;

namespace BondomanShooter.Game.Quest {
    public class PreQuest : BaseQuest {
        private int mobsCount;

        public override void OnObjectSpawned(GameObject obj) {
            if(obj.TryGetComponent(out Mob mob)) {
                mob.OnMobSpawned.AddListener(OnMobSpawned);
                mob.OnMobDeath.AddListener(OnMobDeath);
            }
        }

        public void OnMobSpawned() {
            mobsCount++;
        }

        public void OnMobDeath() {
            mobsCount = Mathf.Max(0, mobsCount - 1);
        }

        public override bool IsFinished() {
            return mobsCount == 0 && GameController.Instance.TimeRemaining >= 0;
        }

        public override bool IsGameOver() {
            return GameController.Instance.TimeRemaining <= 0;
        }
    }
}
