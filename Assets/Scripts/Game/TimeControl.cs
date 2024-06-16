using BondomanShooter.Audio;
using System.Collections;
using UnityEngine;

namespace BondomanShooter.Game {
    public class TimeControl : MonoBehaviour
    {
        public static TimeControl Instance { get; private set; }

        [SerializeField] private string unscaledAudioSourceTag;

        public bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;
                TimeScale = value ? 0f : 1f;
            }
        }

        private float TimeScale
        {
            set
            {
                Time.timeScale = value;
                foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
                {
                    if (source.CompareTag(unscaledAudioSourceTag)) continue;
                    source.pitch = value;
                }
            }
        }

        private bool isPaused;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
        }

        public void Freeze(float duration)
        {
            if (duration <= 0f) return;

            StopAllCoroutines();
            StartCoroutine(TimeFreeze(duration));
        }

        public void LerpBackFromFreeze(float duration) => LerpBackFromScale(0f, duration);

        public void LerpBackFromScale(float scale, float duration)
        {
            if (scale < 0f || duration <= 0f) return;

            StopAllCoroutines();
            StartCoroutine(TimeLerpBackFromScale(scale, duration));
        }

        private IEnumerator TimeFreeze(float duration)
        {
            TimeScale = 0f;
            float elapsed = 0f;
            while(elapsed < duration)
            {
                while (IsPaused) yield return new WaitForEndOfFrame();

                TimeScale = 0f;
                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }
            TimeScale = 1f;
        }

        private IEnumerator TimeLerpBackFromScale(float scale, float duration)
        {
            TimeScale = scale;

            float elapsed = 0f;
            while (elapsed <= duration)
            {
                while (IsPaused) yield return new WaitForEndOfFrame();

                float newScale = Mathf.Lerp(scale, 1f, elapsed / duration);
                TimeScale = newScale;
                elapsed += Time.unscaledDeltaTime;

                yield return new WaitForEndOfFrame();
            }

            TimeScale = 1f;
        }
    }
}
