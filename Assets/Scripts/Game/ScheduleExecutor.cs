using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BondomanShooter.Game {
    public class ScheduleExecutor : MonoBehaviour {
        [SerializeField] private bool executeImmediately;
        [SerializeField] private ScheduleItem[] schedule;

        private void Start() {
            Array.Sort(schedule);
            if(executeImmediately) StartCoroutine(ExecuteSchedule(schedule));
        }

        public void Execute() {
            StopAllCoroutines();
            StartCoroutine(ExecuteSchedule(schedule));
        }

        public void Stop() {
            StopAllCoroutines();
        }

        private IEnumerator ExecuteSchedule(ScheduleItem[] schedule) {
            float startTime = Time.time;
            float lastTimestamp = 0f;

            foreach(ScheduleItem item in schedule) {
                if(item.Timestamp > lastTimestamp) yield return new WaitForSeconds(item.Timestamp - lastTimestamp);

                item.OnExecute.Invoke();
                lastTimestamp = Time.time - startTime;
            }
        }

        [Serializable]
        public class ScheduleItem : IComparable<ScheduleItem> {
            [SerializeField] private float timestamp;
            [SerializeField] private UnityEvent onExecute;

            public float Timestamp => timestamp;
            public UnityEvent OnExecute => onExecute;

            public int CompareTo(ScheduleItem other) => timestamp.CompareTo(other.timestamp);
        }
    }
}
