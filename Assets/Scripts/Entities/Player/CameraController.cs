using UnityEngine;

namespace BondomanShooter.Entities.Player {
    public class CameraController : MonoBehaviour {
        [Header("Camera Follow")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private float followTimeMultiplier = 30f;

        private Vector3 offset;

        private void Start() {
            offset = transform.position - followTarget.position;
        }

        private void Update() {
            if(followTarget != null) {
                Vector3 targetPosition = followTarget.position + offset;
                transform.position = Vector3.Lerp(transform.position, targetPosition, followTimeMultiplier * Time.deltaTime);
            }
        }
    }
}
