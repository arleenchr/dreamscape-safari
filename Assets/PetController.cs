using BondomanShooter.Game;
using BondomanShooter.Items.Weapons;
using BondomanShooter.Structs;
using UnityEngine;
using UnityEngine.AI;

namespace BondomanShooter.Entities.Pets
{
    [RequireComponent(typeof(NavMeshAgent), typeof(WeaponOwner), typeof(Collider))]
    public class PetController : MonoBehaviour, IHealth
    {
        private NavMeshAgent navMeshAgent;
        private Animator animator;

        private int currHealth;

        public WeaponOwner WeaponOwner { get; set; }
        public PetOwner Owner { get; set; }

        public int Health => currHealth;
        public int MaxHealth => maxHealth;

        public NavMeshAgent NavAgent => navMeshAgent;
        public Animator Animator => animator;

        public Transform Target { get; set; }
        public Vector3 MoveTo { get; set; }

        public float lastRefreshTime;
        public bool IsFullHPPet;

        [Header("Pet Attributes")]
        [SerializeField] private StateMachine<PetController> petStateMachine;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float HealPercentage;
        [SerializeField] private float AttackDamage;
        [SerializeField] private float IncreasePercentage;

        public void ApplyHeal()
        {
            if (Owner.TryGetComponent(out IHealth ownerHealth))
            {
                ownerHealth.ApplyDamage(null, (int)(-HealPercentage * ownerHealth.MaxHealth), transform.position, false);
            }
        }

        public void ApplyIncrease()
        {
            if (Owner.TryGetComponent(out WeaponOwner weaponOwner))
            {
                Debug.Log("Found weapon owner");
                weaponOwner.DamageMultFromPets += IncreasePercentage;
            }
        }

        public void UnapplyIncrease()
        {
            if (Owner.TryGetComponent(out WeaponOwner weaponOwner))
            {
                weaponOwner.DamageMultFromPets -= IncreasePercentage;
            }
        }

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

            //GameObject player = GameObject.FindGameObjectWithTag("Player");
            //target = player != null ? player.transform : null;
            animator = GetComponent<Animator>();
            animator.keepAnimatorStateOnDisable = false;
            currHealth = maxHealth;
            WeaponOwner = GetComponent<WeaponOwner>();
        }

        void Start()
        {
            Target = Owner.transform;
            this.ApplyIncrease();
        }

        void Update()
        {
            if (((IHealth)this).IsDead)
            {
                return;
            }

            // Refresh target
            //Target = Target.TryGetComponent(out IHealth targetHealth) && !targetHealth.IsDead ? Target : null;

            // Update state
            petStateMachine.OnUpdate(this);

            if (Time.time - lastRefreshTime >= 2)
            {
                this.ApplyHeal();
                lastRefreshTime = Time.time;
            }
        }


        void OnDeath()
        {
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }

            // Stop and disable nav-mesh agent to prevent blocking other agents
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;

            this.UnapplyIncrease();

            // Fire death animation
            animator.SetTrigger("die");

            // Update Pet Owner
            GameController.Instance.player.PetOwner.UnequipPet(this);
            GameController.Instance.player.PetOwner.DisownPet(this);
        }

        void OnRevive()
        {
            currHealth = MaxHealth;

            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = true;
            }

            // Stop and disable nav-mesh agent to prevent blocking other agents
            navMeshAgent.isStopped = false;
            navMeshAgent.enabled = true;

            Target = Owner.transform;
            this.ApplyIncrease();

            // Fire death animation
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            // Update Pet Owner
            GameController.Instance.player.PetOwner.UnequipPet(this);
            //GameController.Instance.player.PetOwner.DisownPet(this);
        }

        public DamageResult ApplyDamage(WeaponOwner source, int damage, Vector3 location, bool parriable = true)
        {
            if (((IHealth)this).IsDead) return DamageResult.None;

            // Mengurangi health karakter musuh
            currHealth = !IsFullHPPet ? Mathf.Max(0, currHealth - damage) : currHealth;

            if (((IHealth)this).IsDead)
            {
                OnDeath();
            }

            return DamageResult.Hit;
        }

        public void RevivePet()
        {
            OnRevive();
        }

        public void FullHPPet()
        {
            IsFullHPPet = !IsFullHPPet;
        }

        public void KillPet()
        {
            ApplyDamage(null, currHealth, transform.position, false);
        }
    }
}
