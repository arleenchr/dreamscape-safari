using BondomanShooter.Entities.Mobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BondomanShooter.Game.Quest {
    public abstract class BaseQuest : MonoBehaviour {
        [Header("Quest Attributes")]
        [SerializeField] protected string title;
        [SerializeField] protected string description;
        [SerializeField] protected float timeLimit;
        [SerializeField] protected int rewardCoin;

        [Header("Quest Events")]
        [SerializeField] protected UnityEvent onQuestStarted;
        [SerializeField] protected UnityEvent onQuestFinished;

        public string Title => title;
        public string Description => description;
        public float TimeLimit => timeLimit;
        public int RewardCoin => rewardCoin;
        public bool Started => started;

        protected bool started;
        protected bool hasFinished;

        public virtual void StartQuest() {
            if(started) return;
            started = true;
            onQuestStarted.Invoke();
        }

        protected virtual void Update() {
            if(!hasFinished && IsFinished()) {
                hasFinished = true;
                GameController.Instance.Coins += rewardCoin;
                onQuestFinished.Invoke();
            }
        }

        public virtual void OnObjectSpawned(GameObject obj) { }
        public abstract bool IsFinished();
        public abstract bool IsGameOver();
    }
}
