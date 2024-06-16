namespace BondomanShooter.Game.Quest {
    public class Quest4 : BaseQuest {
        public bool IsKongDead { get; set; }

        public override bool IsFinished() {
            return IsKongDead;
        }

        public override bool IsGameOver() {
            return GameController.Instance.player.Health <= 0;
        }
    }
}
