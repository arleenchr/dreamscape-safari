using System.Collections;
using UnityEngine;

namespace BondomanShooter.Game {
    [RequireComponent(typeof(SpriteRenderer))]
    public class ScreenFlash : MonoBehaviour {
        public static ScreenFlash Instance { get; private set; }

        [SerializeField] private float flashTargetAlpha = 0.05f;

        private SpriteRenderer flashRenderer;

        private void Awake() {
            if(Instance != null) Destroy(this); Instance = this;
            flashRenderer = GetComponent<SpriteRenderer>();
        }

        public void FlashFade(float duration) => FlashFade(duration, Color.white);

        public void FlashFade(float duration, Color color) {
            StopAllCoroutines();
            StartCoroutine(DoFlashFade(duration, color));
        }

        public void Flash(float duration) => Flash(duration, Color.white);

        public void Flash(float duration, Color color) {
            StopAllCoroutines();
            StartCoroutine(DoFlash(duration, color));
        }

        private IEnumerator DoFlashFade(float duration, Color color) {
            float elapsed = 0f;
            while(elapsed <= duration) {
                color.a = Mathf.Lerp(flashTargetAlpha, 0f, elapsed / duration);
                flashRenderer.color = color;
                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            flashRenderer.color = Color.clear;
        }

        private IEnumerator DoFlash(float duration, Color color) {
            color.a = flashTargetAlpha;
            flashRenderer.color = color;

            yield return new WaitForSeconds(duration);
            flashRenderer.color = Color.clear;
        }
    }
}
