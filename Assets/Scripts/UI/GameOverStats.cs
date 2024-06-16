using BondomanShooter.Game;
using BondomanShooter.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverStats : MonoBehaviour
{
    public TextMeshProUGUI shotAccuracyValue;
    public TextMeshProUGUI distanceValue;
    public TextMeshProUGUI playtimeValue;
    public TextMeshProUGUI killCountValue;
    public TextMeshProUGUI parryValue;
    public TextMeshProUGUI coinsSpentValue;

    private void Start()
    {
        shotAccuracyValue.text = String.Format("{0:0.00}%", GameController.Instance.ShotAccuracy * 100);
        distanceValue.text = String.Format("{0:0.00}m", GameController.Instance.DistanceTraveled);
        playtimeValue.text = UIController.FormatTime(GameController.Instance.PlayTime, "hms");
        killCountValue.text = GameController.Instance.KillCount.ToString();
        parryValue.text = GameController.Instance.Parry.ToString();
        coinsSpentValue.text = GameController.Instance.CoinsSpent.ToString();
    }
}
