using BondomanShooter.Items.Weapons;
using UnityEngine;

namespace BondomanShooter.Entities.Bullets
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class FireballShot : MonoBehaviour
    {
        public int Damage { get; set; }
        public float Speed { get; set; }
        public float TimeToLive { get; set; }
        public WeaponOwner Owner { get; set; }

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rb.velocity = Speed * transform.forward;
        }

        private void Update()
        {
            // If TTL reaches zero, destroy this bullet
            if (TimeToLive <= 0f) Destroy(gameObject);

            // Update TTL
            TimeToLive -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check collision layer and get the IHealth component of the target
            if (other != null && other.gameObject != Owner.gameObject)
            {
                IHealth targetHealth = other.GetComponentInParent<IHealth>();
                if (targetHealth != null && targetHealth is Component comp && comp.gameObject != Owner.gameObject)
                {
                    // Apply bullet damage to the target
                    Vector3 lastFramePosition = transform.position - 3f * Time.fixedDeltaTime * rb.velocity;
                    DamageResult result = targetHealth.ApplyDamage(Owner, Damage, lastFramePosition);

                    // If the bullet is parried, deflect
                    if (result.HasFlag(DamageResult.Parried))
                    {
                        Vector3 hitNormal = (lastFramePosition - comp.transform.position).normalized;
                        float deflectAngle = Vector3.SignedAngle(transform.forward, hitNormal, Vector3.up);
                        transform.Rotate(0f, 180f + 2f * deflectAngle, 0f);

                        rb.velocity = Speed * transform.forward;
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}
