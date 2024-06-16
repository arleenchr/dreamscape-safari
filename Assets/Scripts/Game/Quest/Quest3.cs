namespace BondomanShooter.Game.Quest {
    public class Quest3 : BaseQuest {
        private bool finished;

        public override bool IsFinished() {
            return finished;
        }

        public override bool IsGameOver() {
            return GameController.Instance.TimeRemaining <= 0 || GameController.Instance.player.Health <= 0;
        }

        public void OnFinishTrigger() {
            finished = true;
        }
    }
}
