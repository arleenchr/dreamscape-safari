using System.Collections;
using UnityEngine;

namespace BondomanShooter.Game {
    [RequireComponent(typeof(Animation))]
    public class LightningFlash : MonoBehaviour {
        [SerializeField] private Vector2 flashIntervalRange;

        private new Animation animation;
        private AudioSource audioSource;

        private void Awake() {
            animation = GetComponent<Animation>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable() {
            StartCoroutine(DoFlash());
        }

        private void OnDisable() {
            StopAllCoroutines();
        }

        private IEnumerator DoFlash() {
            while(true) {
                float interval = Random.Range(flashIntervalRange.x, flashIntervalRange.y);
                yield return new WaitForSeconds(interval);

                // Set rotation to a random angle
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Random.value * 360f, 0f);

                animation.Play();
                if(audioSource != null) audioSource.Play();
            }
        }
    }
}
