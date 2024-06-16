using BondomanShooter.Entities;
using BondomanShooter.Game;
using UnityEngine;

namespace BondomanShooter.Items.Weapons.Types {
    public class DefaultGun : BaseWeapon {
        [Header("Attributes")]
        [SerializeField] private int damage;
        [SerializeField] private float shotInterval;
        [SerializeField] private float shotRange;
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private LayerMask raycastTargetMask;

        [Header("Effects")]
        [SerializeField] private Transform gunBarrelEnd;
        [SerializeField] private ParticleSystem smokeParticleEmitter;
        [SerializeField] private ParticleSystem casingParticleEmitter;

        public override int Damage => (int)(damage * Owner.DamageModifier.Value);
        //public override int Damage => (int)(damage * Owner.DamageMultiplier);

        private LineRenderer lineRenderer;
        private Animator animator;
        private AudioSource gunEffect;

        private bool isShooting;
        private float lastShotTime;

        private void Awake() {
            lineRenderer = GetComponent<LineRenderer>();
            animator = GetComponent<Animator>();
            gunEffect = GetComponent<AudioSource>();
        }

        private void Update() {
            if(isShooting && Time.time > lastShotTime + shotInterval) {
                Vector3 origin = raycastOrigin.position;
                Vector3 direction = Owner.transform.forward;
                Vector3 endpoint = origin + shotRange * direction;

                // Do raycast check
                if(Physics.Raycast(origin, direction, out RaycastHit hit, shotRange, raycastTargetMask)) {
                    // Handle raycast hit
                    // Update line endpoint
                    endpoint = hit.point;

                    // Update stats
                    if (Owner.CompareTag("Player"))
                        GameController.Instance.TotalShots++;

                    if(hit.collider != null && hit.collider.gameObject != Owner.gameObject) {
                        // Check if the target object has an IHealth component in any parent
                        IHealth targetHealth = hit.collider.GetComponentInParent<IHealth>();

                        // Apply weapon damage to the target
                        if (targetHealth != null)
                        {
                            DamageResult res = targetHealth.ApplyDamage(Owner, Damage, hit.point);

                            // Update stats
                            if (Owner.CompareTag("Player") && res.HasFlag(DamageResult.Hit))
                                GameController.Instance.HitShots++;
                        }
                    }
                }

                // Update last shot timestamp
                lastShotTime = Time.time;
                gunEffect.Play();

                // Compute line endpoints for rendering bullets
                if (lineRenderer != null) {
                    lineRenderer.SetPosition(0, gunBarrelEnd.position);
                    lineRenderer.SetPosition(1, endpoint);
                }

                // Trigger shooting animation
                if(animator != null) {
                    animator.SetTrigger("_fire");
                }
            }
        }

        public override void PrimaryAction(bool performing) {
            isShooting = performing;
            
        }

        public void EmitParticles() {
            if(smokeParticleEmitter != null) smokeParticleEmitter.Play();
            if(casingParticleEmitter != null) casingParticleEmitter.Play();
        }
    }
}
