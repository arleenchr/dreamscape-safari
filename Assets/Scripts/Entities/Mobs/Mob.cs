using BondomanShooter.Game;
using BondomanShooter.Items.Weapons;
using BondomanShooter.Structs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BondomanShooter.Entities.Mobs {
    [RequireComponent(typeof(NavMeshAgent), typeof(WeaponOwner), typeof(Collider))]
    public class Mob : MonoBehaviour, IHealth {
        private NavMeshAgent navMeshAgent;
        private WeaponOwner weaponOwner;
        private Animator animator;
        private LootBag lootBag;

        private Transform target;
        private int currHealth;

        public int Health => currHealth;
        public int MaxHealth => maxHealth;

        public NavMeshAgent NavAgent => navMeshAgent;
        public WeaponOwner WeaponOwner => weaponOwner;
        public Animator Animator => animator;

        public UnityEvent OnMobSpawned => onMobSpawned;
        public UnityEvent OnMobDeath => onMobDeath;

        public Transform Target { get; set; }
        public Vector3 MoveTo { get; set; }

        [Header("Enemy Attributes")]
        [SerializeField] private StateMachine<Mob> mobStateMachine;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private AudioSource attackedEffect;
        [SerializeField] private AudioSource deathEffect;
        [SerializeField] private AudioSource roarEffect;

        [Header("Events")]
        [SerializeField] private UnityEvent onMobSpawned;
        [SerializeField] private UnityEvent onMobDeath;

        public AudioSource AttackedAudio => attackedEffect;
        public AudioSource DeathAudio => deathEffect;
        public AudioSource RoarAudio => roarEffect;

        void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            weaponOwner = GetComponent<WeaponOwner>();
            lootBag = GetComponent<LootBag>();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            target = player != null ? player.transform : null;
            if (target != null) { roarEffect.Play(); }
            animator = GetComponent<Animator>();
            currHealth = maxHealth;
        }

        private void Start() {
            onMobSpawned.Invoke();
        }

        void Update() {
            if(((IHealth)this).IsDead) {
                return;
            }

            // Refresh target
            Target = target.TryGetComponent(out IHealth targetHealth) && !targetHealth.IsDead ? target : null;

            // Update state
            mobStateMachine.OnUpdate(this);
        }

        void OnDeath() {
            foreach(Collider c in GetComponentsInChildren<Collider>()) {
                c.enabled = false;
            }

            // Stop weapon action
            weaponOwner.PrimaryAction(false);

            // Stop and disable nav-mesh agent to prevent blocking other agents
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;

            // Spawn loot
            if(lootBag != null) lootBag.InstantiateLoot(transform.position);

            // Invoke death events
            onMobDeath.Invoke();
            deathEffect.Play();

            // Fire death animation
            animator.SetTrigger("die");
        }

        public DamageResult ApplyDamage(WeaponOwner source, int damage, Vector3 location, bool parriable = true) {
            if(((IHealth)this).IsDead) return DamageResult.None;

            if (damage > 0)
            {
                attackedEffect.Play();
            }
            // Mengurangi health karakter musuh
            currHealth = Mathf.Max(0, currHealth - damage);

            if(((IHealth)this).IsDead) {
                OnDeath();

                // Update stats
                if (source.CompareTag("Player"))
                    GameController.Instance.KillCount++;
            }

            return DamageResult.Hit;
        }
    }
}
