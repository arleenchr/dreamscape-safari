using BondomanShooter.Structs;
using System.Collections.Generic;
using UnityEngine;

namespace BondomanShooter.Entities.Mobs {
    public class KongMobStateMachine : StateMachine<Mob> {
        [Header("Movement")]
        [SerializeField] private float nearFarThreshold;
        [SerializeField] private float nearMoveSpeed;
        [SerializeField] private float farMoveSpeed;
        [SerializeField] private float pathRefreshInterval;

        [Header("Retreat")]
        [SerializeField] private Transform[] retreatLocations;

        [Header("Attacks")]
        [SerializeField, Range(0f, 1f)] private float meleeVsRangedRatio;
        [SerializeField] private float attackReconsiderInterval;
        [SerializeField] private float meleeDistance, meleeArc;
        [SerializeField] private float rangedMinDistance, rangedDistance, rangedArc;

        [Header("Summons")]
        [SerializeField] private GameObject footmanPrefab;
        [SerializeField] private int footmanSpawnCount;
        [SerializeField] private float footmanSpawnOffset;
        [SerializeField] private float footmanSpawnInterval;

        public State IdleState { get; private set; }
        public State ApproachState { get; private set; }
        public State AttackMeleeState { get; private set; }
        public State AttackRangedState { get; private set; }
        public State RetreatState { get; private set; }
        public State SummonState { get; private set; }

        public float FootmanSpawnInterval => footmanSpawnInterval;
        public float LastSpawnFootmanTimestamp { get; set; }

        private void Awake() {
            IdleState = new Idle(this);
            ApproachState = new Approach(
                this, nearFarThreshold, nearMoveSpeed, farMoveSpeed, meleeVsRangedRatio,
                meleeDistance, meleeArc, rangedMinDistance, rangedDistance, rangedArc,
                pathRefreshInterval, attackReconsiderInterval
            );
            AttackMeleeState = new AttackMelee(this);
            AttackRangedState = new AttackRanged(this);
            RetreatState = new Retreat(this, retreatLocations, farMoveSpeed);
            SummonState = new Summon(this, footmanPrefab, footmanSpawnCount, footmanSpawnOffset);

            CurrentState = IdleState;
        }

        private void GizmosDrawCircle(float radius) {
            float arcLength = .5f;
            System.Span<Vector3> points = stackalloc Vector3[(int)(2f * Mathf.PI * radius / arcLength)];
            //float angle = 0f;

            for(int i = 0; i < points.Length; i++) {
                float angle = i * arcLength / radius;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                points[i] = transform.position + offset;
            }

            Gizmos.DrawLineStrip(points.ToArray(), true);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            GizmosDrawCircle(nearFarThreshold);

            Gizmos.color = Color.red;
            GizmosDrawCircle(meleeDistance);

            Gizmos.color = new Color(0f, .7f, 0f, 1f);
            GizmosDrawCircle(rangedMinDistance);

            Gizmos.color = Color.green;
            GizmosDrawCircle(rangedDistance);
        }

        public abstract class KongState : State {
            protected KongMobStateMachine KongMachine => Machine as KongMobStateMachine;

            public KongState(KongMobStateMachine stateMachine) {
                Machine = stateMachine;
            }

            public bool ShouldSummonFootman => Time.time > KongMachine.LastSpawnFootmanTimestamp + KongMachine.FootmanSpawnInterval;
        }

        public class Idle : KongState {
            public Idle(KongMobStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(Mob mob) {
                mob.Animator.SetBool("isMoving", false);
            }

            public override State OnUpdate(Mob mob) {
                if(mob.Target == null) return KongMachine.IdleState;

                // Retreat and prepare to summon a footman
                if(ShouldSummonFootman) return KongMachine.RetreatState;

                if(mob.Target != null) return KongMachine.ApproachState;

                return this;
            }
        }

        public class Approach : KongState {
            private readonly float nearFarThreshold, nearMoveSpeed, farMoveSpeed, pathRefreshInterval, attackReconsiderInterval;
            private readonly float meleeVsRangedRatio, meleeDistance, meleeArc, rangedMinDistance, rangedDistance, rangedArc;

            private bool tryDoRanged, actualDoRanged;
            private float lastRefreshTime, lastAttackReconsider;

            public Approach(
                KongMobStateMachine stateMachine,
                float nearFarThreshold, float nearMoveSpeed, float farMoveSpeed, float meleeVsRangedRatio,
                float meleeDistance, float meleeArc, float rangedMinDistance, float rangedDistance, float rangedArc,
                float pathRefreshInterval, float attackReconsiderInterval
            ) : base(stateMachine) {
                this.nearFarThreshold = nearFarThreshold;
                this.nearMoveSpeed = nearMoveSpeed;
                this.farMoveSpeed = farMoveSpeed;

                this.meleeVsRangedRatio = meleeVsRangedRatio;
                this.meleeDistance = meleeDistance;
                this.meleeArc = meleeArc;
                this.rangedMinDistance = rangedMinDistance;
                this.rangedDistance = rangedDistance;
                this.rangedArc = rangedArc;

                this.pathRefreshInterval = pathRefreshInterval;
                this.attackReconsiderInterval = attackReconsiderInterval;
            }

            public override void OnEnter(Mob mob) {
                mob.Animator.SetBool("isMoving", true);
                mob.NavAgent.isStopped = false;
                mob.NavAgent.destination = mob.Target.position;

                // Select attack type to be attempted
                tryDoRanged = actualDoRanged = Random.value > meleeVsRangedRatio;
                mob.WeaponOwner.Select(tryDoRanged ? 1 : 0);

                lastRefreshTime = lastAttackReconsider = Time.time;
            }

            public override State OnUpdate(Mob mob) {
                if(mob.Target == null) return KongMachine.IdleState;

                // Retreat and prepare to summon a footman
                if(ShouldSummonFootman) return KongMachine.RetreatState;

                // Return to idle if path is invalid
                if(mob.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid)) return KongMachine.IdleState;

                Vector3 dir = mob.Target.position - mob.transform.position;
                float sqrDist = dir.sqrMagnitude;

                // Check distance validity
                if(tryDoRanged) {
                    // CANCELED: If too close to do a ranged attack, switch to melee
                    // If too close to do a ranged attack, retreat
                    if(actualDoRanged && sqrDist < rangedMinDistance * rangedMinDistance) {
                        //mob.WeaponOwner.Select(0);
                        //actualDoRanged = false;
                        return KongMachine.RetreatState;
                    }

                    // If the target has got back to ranged distance, switch back to ranged
                    //if(!actualDoRanged && sqrDist > rangedMinDistance * rangedMinDistance) {
                    //    mob.WeaponOwner.Select(1);
                    //    actualDoRanged = true;
                    //}
                }

                // Do attacks
                if(actualDoRanged && sqrDist < rangedDistance * rangedDistance) {
                    Vector2 dir2 = new Vector2(dir.x, dir.z).normalized;
                    Vector2 fwd2 = new(mob.transform.forward.x, mob.transform.forward.z);

                    if(Vector3.Angle(dir2, fwd2) < rangedArc / 2f) {
                        return KongMachine.AttackRangedState;
                    }
                } else if(!actualDoRanged && sqrDist < meleeDistance * meleeDistance) {
                    Vector2 dir2 = new Vector2(dir.x, dir.z).normalized;
                    Vector2 fwd2 = new(mob.transform.forward.x, mob.transform.forward.z);

                    if(Vector3.Angle(dir2, fwd2) < meleeArc / 2f) {
                        return KongMachine.AttackMeleeState;
                    }
                }

                // Set animator walk speed
                if(sqrDist > nearFarThreshold * nearFarThreshold) {
                    mob.Animator.SetFloat("moveSpeed", 1f);
                    mob.NavAgent.speed = farMoveSpeed;
                } else {
                    mob.Animator.SetFloat("moveSpeed", 0f);
                    mob.NavAgent.speed = nearMoveSpeed;
                }

                // Refresh path
                if(Time.time > lastRefreshTime + pathRefreshInterval) {
                    mob.NavAgent.destination = mob.Target.position;
                    lastRefreshTime = Time.time;
                }

                // Reconsider attack type
                if(Time.time > lastAttackReconsider + attackReconsiderInterval) {
                    tryDoRanged = actualDoRanged = Random.value > meleeVsRangedRatio;
                    mob.WeaponOwner.Select(tryDoRanged ? 1 : 0);
                    lastAttackReconsider = Time.time;
                }

                return this;
            }

            public override void OnExit(Mob mob) {
                mob.Animator.SetBool("isMoving", false);
                mob.NavAgent.isStopped = true;
            }
        }

        public class AttackMelee : KongState {
            private bool hasSwitched;

            public AttackMelee(KongMobStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(Mob mob) {
                mob.Animator.SetTrigger("attackMelee");
                mob.WeaponOwner.Select(0);
                mob.WeaponOwner.PrimaryAction(true);

                hasSwitched = false;
            }

            public override State OnUpdate(Mob mob) {
                var stateInfo = mob.Animator.GetCurrentAnimatorStateInfo(0);
                bool isAttackState = stateInfo.IsTag("Attack");

                if(!hasSwitched) hasSwitched = isAttackState;
                if(hasSwitched && !isAttackState) return KongMachine.IdleState;

                return this;
            }

            public override void OnExit(Mob mob) {
                mob.WeaponOwner.PrimaryAction(false);
            }
        }

        public class AttackRanged : KongState {
            private bool hasSwitched;

            public AttackRanged(KongMobStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(Mob mob) {
                mob.Animator.SetTrigger("attackRanged");
                mob.WeaponOwner.Select(1);
                mob.WeaponOwner.PrimaryAction(true);

                hasSwitched = false;
            }

            public override State OnUpdate(Mob mob) {
                var stateInfo = mob.Animator.GetCurrentAnimatorStateInfo(0);
                bool isAttackState = stateInfo.IsTag("Attack");

                if(!hasSwitched) hasSwitched = isAttackState;
                if(hasSwitched && !isAttackState) return KongMachine.IdleState;

                return this;
            }

            public override void OnExit(Mob mob) {
                mob.WeaponOwner.PrimaryAction(false);
                //mob.Animator.SetBool("isAttackingRanged", false);
            }
        }

        public class Retreat : KongState {
            private readonly Transform[] retreatLocations;
            private readonly float runMoveSpeed;

            public Retreat(KongMobStateMachine stateMachine, Transform[] retreatLocations, float runMoveSpeed) : base(stateMachine) {
                this.retreatLocations = retreatLocations;
                this.runMoveSpeed = runMoveSpeed;
            }

            public override void OnEnter(Mob mob) {
                float currentMaxSqrDist = float.NegativeInfinity;
                Transform currentFurthestLoc = null;

                // Find the retreat location furthest from the player
                foreach(Transform loc in retreatLocations) {
                    float sqrDist = Vector3.SqrMagnitude(mob.Target.position - loc.position);
                    if(sqrDist > currentMaxSqrDist) {
                        currentFurthestLoc = loc;
                        currentMaxSqrDist = sqrDist;
                    }
                }

                mob.NavAgent.isStopped = false;
                mob.NavAgent.destination = currentFurthestLoc.position;
                mob.NavAgent.speed = runMoveSpeed;

                mob.Animator.SetBool("isMoving", true);
                mob.Animator.SetFloat("moveSpeed", 1f);
            }

            public override State OnUpdate(Mob mob) {
                // Return to idle if path is invalid
                if(mob.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid)) return KongMachine.IdleState;

                // If destination has been reached...
                if(mob.NavAgent.remainingDistance <= mob.NavAgent.stoppingDistance) {
                    // Summon a footman if should
                    if(ShouldSummonFootman) return KongMachine.SummonState;

                    // Else, go back to idle
                    return KongMachine.IdleState;
                }

                return this;
            }

            public override void OnExit(Mob mob) {
                mob.NavAgent.isStopped = true;
                mob.Animator.SetBool("isMoving", false);
            }
        }

        public class Summon : KongState {
            private readonly float footmanSpawnOffset;
            private readonly int footmanSpawnCount;
            private readonly GameObject footmanPrefab;

            public Summon(KongMobStateMachine stateMachine, GameObject footmanPrefab, int footmanSpawnCount, float footmanSpawnOffset) : base(stateMachine) {
                this.footmanPrefab = footmanPrefab;
                this.footmanSpawnCount = footmanSpawnCount;
                this.footmanSpawnOffset = footmanSpawnOffset;
            }

            public override void OnEnter(Mob mob) {
                mob.Animator.SetBool("isSummoning", true);
                mob.RoarAudio.Play();
                KongMachine.LastSpawnFootmanTimestamp = Time.time;
            }

            public override State OnUpdate(Mob mob) {
                if(mob.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
                    for(int i = 0; i < footmanSpawnCount; i++) {
                        Vector3 offsetDir = Quaternion.Euler(0f, i * 360f / footmanSpawnCount, 0f) * mob.transform.forward;
                        Instantiate(footmanPrefab, mob.transform.position + footmanSpawnOffset * offsetDir, mob.transform.rotation);
                    }
                    return KongMachine.ApproachState;
                }
                return this;
            }

            public override void OnExit(Mob mob) {
                mob.Animator.SetBool("isSummoning", false);
            }
        }
    }
}
