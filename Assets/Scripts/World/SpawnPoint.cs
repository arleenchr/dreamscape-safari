using UnityEngine;
using UnityEngine.Events;

namespace BondomanShooter.World {
    public class SpawnPoint : MonoBehaviour {
        [SerializeField] private GameObject defaultSpawnObject;
        [SerializeField] private UnityEvent<GameObject> onSpawn;

        public void SpawnDefault() {
            if(defaultSpawnObject == null) return;
            GameObject obj = Instantiate(defaultSpawnObject, transform.position, transform.rotation);
            onSpawn.Invoke(obj);
        }

        public void Spawn(GameObject obj) {
            Instantiate(obj, transform.position, transform.rotation);
            onSpawn.Invoke(obj);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, .5f);
        }
    }
}
