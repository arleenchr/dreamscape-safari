using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string saveName;
    public DateTime time;
    public int Coins;
    public int HitShots;
    public int TotalShots;
    public float DistanceTraveled;
    public float PlayTime;
    public int KillCount;
    public int Parry;
    public int CoinsSpent;
    public float timer;
    public string CurrScene;

    public GameData()
    {
        this.saveName = string.Empty;
        this.time = DateTime.Now;
        this.Coins = 0;
        this.HitShots = 0;
        this.TotalShots = 0;
        this.DistanceTraveled = 0;
        this.PlayTime = 0;
        this.KillCount = 0;
        this.Parry = 0;
        this.CoinsSpent = 0;
        this.timer = 0;
        this.CurrScene = string.Empty;
    }
}
