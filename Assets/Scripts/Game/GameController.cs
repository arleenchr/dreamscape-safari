using BondomanShooter.Entities;
using BondomanShooter.Entities.Player;
using BondomanShooter.Game.Quest;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BondomanShooter.Game
{
    public class GameController : MonoBehaviour, IDataPersistence
    {
        public PlayerController player;
        public BaseQuest quest;
        public BaseQuest lounge;
        public GameObject darkenOverlayPanel;
        public GameObject gameOverPanel;
        public GameObject shopPanel;

        public static GameController Instance { get; private set; }
        public float TimeRemaining { get; set; }
        public int Coins { 
            get => /*IsMotherlode ? int.MaxValue :*/ coins; 
            set => coins = value; 
        }
        public float ShotAccuracy => TotalShots == 0 ? 0 : ((float)HitShots / TotalShots);
        public int HitShots { get; set; }
        public int TotalShots { get; set; }
        public float DistanceTraveled { get; set; }
        public float PlayTime { get; set; }
        public int KillCount { get; set; }
        public int Parry { get; set; }
        public int CoinsSpent { get; set; }

        public bool IsMotherlode { get; set; }
        public GameObject cheatOrbPrefab;

        public bool QuestStarted => quest != null && quest.Started;

        private float startTime;
        private float endTime;

        private bool playTimer;

        private int coins;

        void Awake()
        {
            Instance = this;
            if (quest != null)
            {
                startTime = Time.time;
                playTimer = true;
            }
        }

        private void Start()
        {
            //PlayerPrefs.SetInt("Coins", 0);
            //Coins = PlayerPrefs.GetInt("Coins", 0);
            //HitShots = PlayerPrefs.GetInt("HitShots", 0);
            //TotalShots = PlayerPrefs.GetInt("TotalShots", 0);
            //DistanceTraveled = PlayerPrefs.GetFloat("DistanceTraveled", 0f);
            //PlayTime = PlayerPrefs.GetFloat("PlayTime", 0f);
            //KillCount = PlayerPrefs.GetInt("KillCount", 0);
            //Parry = PlayerPrefs.GetInt("Parry", 0);
            //CoinsSpent = PlayerPrefs.GetInt("CoinsSpent", 0);
            DataPersistenceManager.Instance.ApplyCurrentData();

            if (quest!=null)
                TimeRemaining = quest.TimeLimit;
            if (lounge != null)
                TimeRemaining = lounge.TimeLimit;

            Debug.Log(Coins);
        }

        void Update()
        {
            if (quest != null)
            {
                // Quest
                // if game over or quest finished
                //if (!IsGameOver() && !IsQuestFinished())
                //    endTime = Time.time;

                if (QuestStarted)
                {
                    if (!IsGameOver() && !IsQuestFinished())
                    {
                        TimeRemaining -= Time.deltaTime;
                    }
                    else
                    {
                        if (IsQuestFinished())
                        {
                            //Coins += quest.RewardCoin;
                            Debug.Log(Coins);
                        }
                        else if (IsGameOver())
                        {
                            StopPlayTimer();
                            darkenOverlayPanel.SetActive(true);
                            gameOverPanel.SetActive(true);
                        }
                        //PlayerPrefs.SetInt("Coins", Coins);
                        //PlayerPrefs.SetInt("HitShots", HitShots);
                        //PlayerPrefs.SetInt("TotalShots", TotalShots);
                        //PlayerPrefs.SetFloat("DistanceTraveled", DistanceTraveled);
                        //PlayerPrefs.SetFloat("PlayTime", PlayTime);
                        //PlayerPrefs.SetInt("KillCount", KillCount);
                        //PlayerPrefs.SetInt("Parry", Parry);
                        //PlayerPrefs.SetInt("CoinsSpent", CoinsSpent);
                        DataPersistenceManager.Instance.StoreCurrentState();
                    }
                }
            }
            if (lounge != null)
            {
                //Debug.Log("Lounge");
                //if (QuestStarted)
                //{
                    //Debug.Log("lounge started");
                // Lounge or story mode
                if (!IsLoungeFinished())
                {
                    //Debug.Log("Lounge on going");
                    TimeRemaining -= Time.deltaTime;
                }
                else
                {
                    //Debug.Log("Lounge finished");
                    StopPlayTimer();
                    if (shopPanel.activeSelf)
                        darkenOverlayPanel.SetActive(!darkenOverlayPanel.activeSelf);
                    shopPanel.SetActive(false);
                }
                //}
            }
        }

        public void StopPlayTimer()
        {
            if (playTimer)
            {
                endTime = Time.time;
                playTimer = false;

                PlayTime += endTime - startTime;
            }
        }

        public bool IsQuestFinished()
        {
            return quest == null || quest.IsFinished();
        }

        public bool IsLoungeFinished()
        {
            return lounge == null || lounge.IsFinished();
        }

        public bool IsGameOver()
        {
            return ((IHealth)player).IsDead || (quest != null && quest.IsGameOver());
        }

        public void LoadNextScene(string nextScene)
        {
            DataPersistenceManager.Instance.ApplyCurrentData();
            SceneManager.LoadScene(nextScene);
        }
        
        public void Motherlode()
        {
            Debug.Log(IsMotherlode);
            IsMotherlode = !IsMotherlode;
        }

        public void CheatOrb()
        {
            Instantiate(cheatOrbPrefab, player.transform.position, Quaternion.identity);
        }

        public void LoadData(GameData gameData)
        {
            this.Coins = gameData.Coins;
            this.HitShots = gameData.HitShots;
            this.TotalShots = gameData.TotalShots;
            this.DistanceTraveled = gameData.DistanceTraveled;
            this.PlayTime = gameData.PlayTime;
            this.KillCount = gameData.KillCount;
            this.Parry = gameData.Parry;
            this.CoinsSpent = gameData.CoinsSpent;
            this.TimeRemaining = gameData.timer;
        }

        public void SaveData(ref GameData gameData)
        {
            gameData.Coins = this.Coins;
            gameData.HitShots = this.HitShots;
            gameData.TotalShots = this.TotalShots;
            gameData.DistanceTraveled = this.DistanceTraveled;
            gameData.PlayTime = this.PlayTime;
            gameData.KillCount = this.KillCount;
            gameData.Parry = this.Parry;
            gameData.CoinsSpent = this.CoinsSpent;
            gameData.timer = this.TimeRemaining;
        }
    }
}
