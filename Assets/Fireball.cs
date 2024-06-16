using BondomanShooter.Entities.Bullets;
using UnityEngine;

namespace BondomanShooter.Items.Weapons.Types
{
    public class Fireball : BaseWeapon
    {
        [Header("Attributes")]
        [SerializeField] private int damagePerBuckshot;
        [SerializeField] private float shotInterval;
        [SerializeField] private float shotRange;
        [SerializeField] private float shotMuzzleSpeed;
        [SerializeField] private float shotSpread;
        [SerializeField] private int shotCount;
        [SerializeField] private Transform bulletOrigin;
        [SerializeField] private FireballShot fireball;

        [Header("Effects")]
        [SerializeField] private ParticleSystem smokeParticleEmitter;
        [SerializeField] private ParticleSystem casingParticleEmitter;

        public override int Damage => (int)(damagePerBuckshot * Owner.DamageModifier.Value);
        //public override int Damage => (int)(damagePerBuckshot * Owner.DamageMultiplier);
        private Animator animator;

        private bool isShooting;
        private float lastShotTime;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (isShooting && Time.time > lastShotTime + shotInterval)
            {
                Vector3 origin = bulletOrigin.position;
                Vector3 directionCenter = Owner.transform.forward;

                // For each buckshot instance...
                for (int i = 0; i < shotCount; ++i)
                {
                    // Instantiate buckshot prefab and set attributes
                    Vector3 direction = Quaternion.Euler(-40f, -40f, -40f) * Owner.transform.forward;

                    GameObject instance = Instantiate(fireball.gameObject, origin, Quaternion.FromToRotation(Vector3.forward, direction));
                    FireballShot fireballshot = instance.GetComponent<FireballShot>();
                    fireballshot.Damage = Damage;
                    fireballshot.Speed = shotMuzzleSpeed;
                    fireballshot.Owner = Owner;
                    fireballshot.TimeToLive = shotRange / shotMuzzleSpeed;
                }

                // Update last shot timestamp
                lastShotTime = Time.time;

                // Trigger shooting animation
                if (animator != null) animator.SetTrigger("_fire");
            }
        }

        public override void PrimaryAction(bool performing)
        {
            isShooting = performing;
        }

        public void EmitMuzzleParticles()
        {
            if (smokeParticleEmitter != null) smokeParticleEmitter.Play();
        }

        public void EmitEjectorParticles()
        {
            if (casingParticleEmitter != null) casingParticleEmitter.Play();
        }
    }
}
