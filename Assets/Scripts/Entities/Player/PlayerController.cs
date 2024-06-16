using BondomanShooter.Entities.Pets;
using BondomanShooter.Game;
using BondomanShooter.Items.Weapons;
using BondomanShooter.Structs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BondomanShooter.Entities.Player {
    [RequireComponent(typeof(CharacterController), typeof(WeaponOwner))]
    public class PlayerController : MonoBehaviour, IHealth, IHasSpeedModifier {
        [Header("Player Attributes")]
        [SerializeField] private int maxHealth;

        [Header("Player Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float moveAcceleration = 12f;
        [SerializeField] private float aimTimeMultiplier = 30f;
        [SerializeField] private AudioSource footstepEffect;
        [SerializeField] private AudioSource deathEffect;

        [Header("Effects")]
        [SerializeField] private float parryFreezeDuration = 0.2f;
        [SerializeField] private ParticleSystem parryParticleEmitter;
        [SerializeField] private AudioSource parryEffect;

        public int Health { get; private set; }
        public int MaxHealth => maxHealth;
        public ModifierStack<float> SpeedModifier { get; } = new ModifierStack<float>(1f);
        //public float SpeedMultiplier { get; set; } = 1f;
        public float RemainingSpeedTime { get; set; }
        public bool IsNoDamage { get; set; }
        public bool IsX2Speed { get; set; }

        public WeaponOwner WeaponOwner => weaponOwner;
        public PetOwner PetOwner => petOwner;

        private CharacterController characterController;
        private WeaponOwner weaponOwner;
        private PetOwner petOwner;

        private Animator animator;
        private Camera mainCamera;
        private PlayerInput playerInput;

        private Vector3 planeVelocity, gravityVelocity;

        private Vector2 inputMove;
        private Vector2 inputAim;
        private (bool changed, bool performing) inputPrimaryAction;

        private void Awake() {
            // Get referenced components
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            weaponOwner = GetComponent<WeaponOwner>();
            petOwner = GetComponent<PetOwner>();

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            playerInput = GameObject.FindGameObjectWithTag("InputController").GetComponent<PlayerInput>();

            // Set initial values
            Health = maxHealth;
        }

        private void Update() {
            // Do not update anything if the player is dead
            if(((IHealth)this).IsDead) return;

            // Rotate player character to look at the target aim direction
            if(inputAim != Vector2.zero) {
                // Flip x and y because we want to calculate CW angle to the local y-axis (global z-axis)
                float targetAngle = Mathf.Atan2(inputAim.x, inputAim.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(
                    0f,
                    Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetAngle, aimTimeMultiplier * Time.deltaTime),
                    0f
                );
            }

            // Update velocity and position
            // Do half acceleration before movement and half acceleration after to even out errors due to low framerate
            // Compute pre-update velocity
            Vector3 worldSpaceMove = new(inputMove.x, 0f, inputMove.y);
            planeVelocity += Time.deltaTime / 2f * moveAcceleration * (moveSpeed * SpeedModifier.Value * (IsX2Speed ? 2 : 1) * worldSpaceMove - planeVelocity);
            gravityVelocity += Time.deltaTime / 2f * Physics.gravity;

            // Move player and compute post-update velocity
            CollisionFlags flags = characterController.Move(Time.deltaTime * (planeVelocity + gravityVelocity));
            GameController.Instance.DistanceTraveled += Time.deltaTime * planeVelocity.magnitude;

            planeVelocity += Time.deltaTime / 2f * moveAcceleration * (moveSpeed * SpeedModifier.Value * (IsX2Speed ? 2 : 1) * worldSpaceMove - planeVelocity);

            // Reset fall velocity if the player collides with something below, otherwise add the rest of acceleration due to gravity
            if(flags.HasFlag(CollisionFlags.CollidedBelow)) {
                gravityVelocity = Vector3.zero;
            } else {
                gravityVelocity += Time.deltaTime / 2f * Physics.gravity;
            }

            // Handle weapon actions, such as switching, using, etc.
            // Handle weapon uses
            if(inputPrimaryAction.changed) {
                weaponOwner.PrimaryAction(inputPrimaryAction.performing);
                inputPrimaryAction.changed = false;
            }

            // Set animator parameters
            if(animator) {
                animator.SetBool("_isMoving", planeVelocity.sqrMagnitude > 0.1f);
            }

            // Reduce speed booster remaining time
            if(RemainingSpeedTime > 0f) {
                RemainingSpeedTime -= Time.deltaTime;
            }

            // Set speed to initial speed
            if(RemainingSpeedTime <= 0f) {
                //SpeedMultiplier = 1f;
            }
        }

        private void HandleParry() {
            GameController.Instance.Parry++;

            if(ScreenFlash.Instance != null) ScreenFlash.Instance.Flash(parryFreezeDuration);
            if(TimeControl.Instance != null) TimeControl.Instance.Freeze(parryFreezeDuration);
            if(parryParticleEmitter != null) parryParticleEmitter.Play();
            if(parryEffect != null) parryEffect.Play();
        }

        private void HandleDeath() {
            // Set animator parameters
            weaponOwner.PrimaryAction(false);
            if(animator) animator.SetBool("_isDead", true);
        }

        public DamageResult ApplyDamage(WeaponOwner source, int damage, Vector3 location, bool parriable = true) {
            // Reduce health by the given amount, down to zero
            if(((IHealth)this).IsDead) return DamageResult.None;

            // Handle parries
            if(parriable && weaponOwner.IsParrying) {
                ParryWeapon parryWeapon = weaponOwner.SelectedWeapon as ParryWeapon;

                // Parry if attack location is within the parry arc
                float angle = Vector3.SignedAngle(transform.forward, location - transform.position, transform.up);
                if(Mathf.Abs(angle) < parryWeapon.ParryArc / 2f) {
                    HandleParry();
                    return DamageResult.Parried;
                }
            }

            Health = !IsNoDamage ? Mathf.Clamp(Health - damage, 0, maxHealth) : Health;
            if(((IHealth)this).IsDead) HandleDeath();

            return DamageResult.Hit;
        }


        public void OnInputMove(InputAction.CallbackContext ctx) {
            inputMove = ctx.ReadValue<Vector2>();
        }

        public void OnInputAim(InputAction.CallbackContext ctx) {
            switch(playerInput.currentControlScheme) {
                case "KeyboardMouse":
                    // Value came from a mouse
                    Vector2 mousePos = ctx.ReadValue<Vector2>();
                    Ray ray = mainCamera.ScreenPointToRay(mousePos);

                    float t = ray.origin.y - transform.position.y;
                    Vector3 aimTarget = ray.GetPoint(t * (ray.direction / ray.direction.y).magnitude);

                    inputAim = new Vector2(aimTarget.x - transform.position.x, aimTarget.z - transform.position.z);
                    break;
                case "Joystick":
                case "Gamepad":
                    // Value came from a joystick or gamepad
                    inputAim = ctx.ReadValue<Vector2>();
                    break;
                default:
                    Debug.LogError($"Unknown control scheme {playerInput.currentControlScheme}");
                    break;
            }
        }

        public void OnInputPrimaryAction(InputAction.CallbackContext ctx) {
            if(ctx.performed != inputPrimaryAction.performing) {
                inputPrimaryAction = (true, ctx.performed);
            }
        }

        public void OnFootstep()
        {
            footstepEffect.Play();
        }

        public void OnDeath()
        {
            deathEffect.Play();
        }

        public void OnPlayerInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                float interactRange = 2f;
                Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
                foreach (Collider collider in colliderArray)
                {
                    if(collider.TryGetComponent(out IInteractable inter))
                    {
                        inter.OnInteract();
                    }
                }
            }
        }

        public void NoDamage()
        {
            IsNoDamage = !IsNoDamage;
        }

        public void X2Speed()
        {
            IsX2Speed = !IsX2Speed;
        }
    }
}
