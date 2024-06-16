using UnityEngine;

namespace BondomanShooter.Game.Quest {
    public class Quest2 : BaseQuest {
        public void DestroyAllMobs() {
            if(!IsFinished()) return;

            foreach(GameObject mob in GameObject.FindGameObjectsWithTag("Mob")) Destroy(mob);
            foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) Destroy(bullet);
        }

        public override bool IsFinished() {
            return GameController.Instance.TimeRemaining <= 0;
        }

        public override bool IsGameOver() {
            return GameController.Instance.player.Health <= 0;
        }
    }
}
