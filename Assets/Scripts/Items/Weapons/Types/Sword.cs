using BondomanShooter.Entities;
using System.Collections;
using UnityEngine;

namespace BondomanShooter.Items.Weapons.Types {
    [RequireComponent(typeof(Collider))]
    public class Sword : ParryWeapon {
        [Header("Attributes")]
        [SerializeField] private int damage;
        [SerializeField] private float swingInterval;
        [SerializeField] private Vector2 swingTriggerTimeframe;

        [Header("Effects")]
        [SerializeField] private TrailRenderer trailRenderer;

        public override int Damage => (int)(damage * Owner.DamageModifier.Value);
        //public override int Damage => (int)(damage * Owner.DamageMultiplier);

        private Collider attackTrigger;
        private AudioSource swordSwingEffect;

        private bool performing;
        private bool wasPerforming;
        private float lastSwingTime;

        private void Awake() {
            attackTrigger = GetComponent<Collider>();
            swordSwingEffect = GetComponent<AudioSource>();
        }

        private void Update() {
            // Only do attacks on the rising edge of performing
            if(!wasPerforming && performing && Time.time > lastSwingTime + swingInterval) {
                // Trigger attack animation
                StartCoroutine(DoAttack());

                lastSwingTime = Time.time;
            }

            wasPerforming = performing;
        }

        private IEnumerator DoAttack() {
            // Trigger animation on the owner
            Owner.OwnerAnimator.SetTrigger("_attack");
            yield return new WaitForSeconds(swingTriggerTimeframe.x);

            // Enable attack trigger
            SetEnableAttackTrigger(true);
            if (swordSwingEffect != null) swordSwingEffect.Play();
            if(trailRenderer != null) trailRenderer.emitting = true;
            yield return new WaitForSeconds(swingTriggerTimeframe.y - swingTriggerTimeframe.x);

            // Disable attack trigger
            SetEnableAttackTrigger(false);
            if(trailRenderer != null) trailRenderer.emitting = false;
        }

        private void OnTriggerEnter(Collider other) {
            // Check collision layer and get the IHealth component of the target
            if(other != null && other.gameObject != Owner.gameObject) {
                IHealth targetHealth = other.GetComponentInParent<IHealth>();
                if(targetHealth != null && targetHealth is Component comp && comp.gameObject != Owner.gameObject) {
                    // Apply melee damage to the target
                    targetHealth.ApplyDamage(Owner, Damage, Owner.transform.position);
                }
            }
        }

        public override void PrimaryAction(bool performing) {
            this.performing = performing;
        }

        public void SetEnableAttackTrigger(bool value) {
            attackTrigger.enabled = value;
            Parrying = canParry && value;
        }
    }
}
