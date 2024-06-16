using UnityEngine;

namespace BondomanShooter.Game {
    [RequireComponent(typeof(Animation))]
    public class AnimationEventListener : MonoBehaviour {
        private new Animation animation;

        private void Awake() {
            animation = GetComponent<Animation>();
        }

        public void PlayAnimation() {
            animation.Play();
        }

        public void StopAnimation() {
            animation.Stop();
        }
    }
}
