using BondomanShooter.Entities.Player;
using UnityEngine;

namespace BondomanShooter.Entities.Collectibles {
    public abstract class Orb : MonoBehaviour {
        [SerializeField] private float timeToLive;
        [SerializeField] private AudioSource orbEffect;

        private float remainingTime;

        private void Start() {
            remainingTime = timeToLive;
        }

        private void Update() {
            remainingTime -= Time.deltaTime;
            if(remainingTime <= 0) {
                Destroy(gameObject);
            }
        }

        public virtual void ApplyEffect(PlayerController player) {
            if(TryGetComponent(out Renderer renderer)) renderer.enabled = false;
            Destroy(gameObject, 1f);
        }

        void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag("Player")) {
                orbEffect.Play();
                ApplyEffect(other.GetComponent<PlayerController>());

                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
