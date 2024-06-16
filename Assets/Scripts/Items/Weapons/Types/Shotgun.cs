using BondomanShooter.Entities.Bullets;
using BondomanShooter.Game;
using UnityEngine;

namespace BondomanShooter.Items.Weapons.Types {
    public class Shotgun : BaseWeapon {
        [Header("Attributes")]
        [SerializeField] private int damagePerBuckshot;
        [SerializeField] private float shotInterval;
        [SerializeField] private float shotRange;
        [SerializeField] private float shotMuzzleSpeed;
        [SerializeField] private float shotSpread;
        [SerializeField] private int shotCount;
        [SerializeField] private Transform bulletOrigin;
        [SerializeField] private ShotgunBuckshot buckshotPrefab;

        [Header("Effects")]
        [SerializeField] private ParticleSystem smokeParticleEmitter;
        [SerializeField] private ParticleSystem casingParticleEmitter;

        public override int Damage => (int)(damagePerBuckshot * Owner.DamageModifier.Value);
        //public override int Damage => (int)(damagePerBuckshot * Owner.DamageMultiplier);
        private Animator animator;

        private bool wasShooting;
        private bool isShooting;
        private float lastShotTime;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Update() {
            if(!wasShooting && isShooting && Time.time > lastShotTime + shotInterval) {
                Vector3 origin = bulletOrigin.position;
                Vector3 directionCenter = Owner.transform.forward;
                
                // For each buckshot instance...
                for(int i = 0; i < shotCount; ++i) {
                    // Instantiate buckshot prefab and set attributes
                    Vector3 direction = Quaternion.Euler(0f, Random.Range(-shotSpread / 2f, shotSpread / 2f), 0f) * directionCenter;

                    GameObject instance = Instantiate(buckshotPrefab.gameObject, origin, Quaternion.FromToRotation(Vector3.forward, direction));
                    ShotgunBuckshot buckshot = instance.GetComponent<ShotgunBuckshot>();
                    buckshot.Damage = Damage;
                    buckshot.Speed = shotMuzzleSpeed;
                    buckshot.Owner = Owner;
                    buckshot.TimeToLive = shotRange / shotMuzzleSpeed;
                }

                // Update last shot timestamp
                lastShotTime = Time.time;

                // Update stats
                if (Owner.CompareTag("Player"))
                    GameController.Instance.TotalShots += shotCount;

                // Trigger shooting animation
                if(animator != null) animator.SetTrigger("_fire");
            }

            wasShooting = isShooting;
        }

        public override void PrimaryAction(bool performing) {
            isShooting = performing;
        }

        public void EmitMuzzleParticles() {
            if(smokeParticleEmitter != null) smokeParticleEmitter.Play();
        }

        public void EmitEjectorParticles() {
            if(casingParticleEmitter != null) casingParticleEmitter.Play();
        }
    }
}
