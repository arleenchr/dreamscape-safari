using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BondomanShooter.Game.Quest
{
    public class Lounge : BaseQuest
    {
        public override bool IsFinished()
        {
            return GameController.Instance.TimeRemaining <= 0;
        }
        public override bool IsGameOver()
        {
            return false;
        }
    }
}