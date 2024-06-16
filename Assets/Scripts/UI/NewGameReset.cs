using BondomanShooter.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameReset : MonoBehaviour
{
    public void ResetStats()
    {
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("HitShots", 0);
        PlayerPrefs.SetInt("TotalShots", 0);
        PlayerPrefs.SetFloat("DistanceTraveled", 0f);
        PlayerPrefs.SetFloat("PlayTime", 0f);
        PlayerPrefs.SetInt("KillCount", 0);
        PlayerPrefs.SetInt("Parry", 0);
        PlayerPrefs.SetInt("CoinsSpent", 0);
    }
}
