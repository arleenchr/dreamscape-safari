using BondomanShooter.Structs;
using System.Reflection;
using UnityEngine;

namespace BondomanShooter.Entities.Mobs {
	public class FollowMobStateMachine : StateMachine<Mob> {
		[Header("Properties")]
		[SerializeField] protected float startChaseRange;
		[SerializeField] protected float stopChaseRange;
		[SerializeField] protected float attackRange;
		[SerializeField, Range(0f, 360f)] protected float attackArc;
		[SerializeField] protected int retreatAttempts;
		[SerializeField] protected float retreatDistance;
		[SerializeField] protected float maxRetreatDuration;

		[Header("Attributes")]
		[SerializeField] protected float pathRefreshInterval;

		public State IdleState { get; private set; }
		public State MoveState { get; private set; }
		public State AttackState { get; private set; }
		public State RetreatState { get; private set; }

		private void Awake() {
            IdleState = new Idle(this, startChaseRange);
            MoveState = new Move(this, stopChaseRange, attackRange, attackArc, pathRefreshInterval);
            AttackState = new Attack(this);
			RetreatState = new Retreat(this, retreatAttempts, retreatDistance, maxRetreatDuration);

            CurrentState = IdleState;
        }

		public abstract class FollowMobState : State {
			protected FollowMobStateMachine FollowMachine => (FollowMobStateMachine)Machine;

			public FollowMobState(FollowMobStateMachine stateMachine) {
				Machine = stateMachine;
			}
		}

		public class Idle : FollowMobState {
			public float startChaseRange;

			public Idle(FollowMobStateMachine stateMachine, float startChaseRange) : base(stateMachine) {
				this.startChaseRange = startChaseRange;
			}

			public override void OnEnter(Mob mob) {
				mob.Animator.SetTrigger("idle");
			}

			public override State OnUpdate(Mob mob) {
				// Do nothing if there is no target
				if(mob.Target == null) return this;

				float sqrDist = (mob.Target.position - mob.transform.position).sqrMagnitude;

				// If target has entered the chase range, start chasing
				if(sqrDist < startChaseRange * startChaseRange) {
					return FollowMachine.MoveState;
				}

				return this;
			}
		}

		public class Move : FollowMobState {
			public float stopChaseRange, attackRange, attackArc, pathRefreshInterval;

			private float lastRefreshTime;

			public Move(FollowMobStateMachine stateMachine, float stopChaseRange, float attackRange, float attackArc, float pathRefreshInterval) : base(stateMachine) {
				this.stopChaseRange = stopChaseRange;
				this.attackRange = attackRange;
				this.attackArc = attackArc;
				this.pathRefreshInterval = pathRefreshInterval;
			}

			public override void OnEnter(Mob mob) {
				mob.Animator.SetTrigger("move");

				// Compute path to follow target
				mob.NavAgent.isStopped = false;
				mob.NavAgent.destination = mob.Target.position;
				lastRefreshTime = Time.time;
			}

			public override State OnUpdate(Mob mob) {
				// If the target is null, stop chasing
				if(mob.Target == null) return FollowMachine.IdleState;

				Vector3 dir = mob.Target.position - mob.transform.position;
				float sqrDist = dir.sqrMagnitude;

                // If the distance to the target reaches the attack range, check attack arc
                if(sqrDist < attackRange * attackRange) {
                    Vector2 dir2 = new Vector2(dir.x, dir.z).normalized;
					Vector2 fwd2 = new(mob.transform.forward.x, mob.transform.forward.z);

					// If target is within the attack arc, begin attacking
					if(Vector2.Angle(dir2, fwd2) < attackArc / 2f) {
						return FollowMachine.AttackState;
					}
				}

				// If no path is found, stop chasing
				if(mob.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid)) return FollowMachine.IdleState;

				// On every path refresh...
				if(Time.time > lastRefreshTime + pathRefreshInterval) {
					// If the target has left the maximum chase range, stop chasing
					if(sqrDist > stopChaseRange * stopChaseRange) {
						return FollowMachine.IdleState;
					}

					// Recompute path to follow target
					mob.NavAgent.destination = mob.Target.position;
					lastRefreshTime = Time.time;
				}

				return this;
			}

            public override void OnExit(Mob mob) {
				mob.NavAgent.isStopped = true;
            }
        }

		public class Attack : FollowMobState {
			public Attack(FollowMobStateMachine stateMachine) : base(stateMachine) { }

			public override void OnEnter(Mob mob) {
				mob.Animator.SetTrigger("attack");

				// Use weapon
				mob.WeaponOwner.PrimaryAction(true);
			}

			public override State OnUpdate(Mob mob) {
				// If animation is finished, return to idle state
				if(mob.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
					mob.WeaponOwner.PrimaryAction(false);
					return FollowMachine.RetreatState;
				}

				return this;
			}
		}

		public class Retreat : FollowMobState {
			public int attempts;
			public float retreatDistance, maxRetreatDuration;

			private float startRetreatTime;

			public Retreat(FollowMobStateMachine stateMachine, int attempts, float retreatDistance, float maxRetreatDuration) : base(stateMachine) {
				this.attempts = attempts;
				this.retreatDistance = retreatDistance;
				this.maxRetreatDuration = maxRetreatDuration;
			}

            public override void OnEnter(Mob mob) {
				mob.Animator.SetTrigger("move");

				// Try to find a random retreat direction
				int i = 0;
				do {
					float angle = Random.Range(0f, 2f * Mathf.PI);
					Vector3 direction = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

					mob.NavAgent.destination = mob.transform.position + retreatDistance * direction;
				} while(++i < attempts && mob.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid));

				// If a retreat path is found, enable navigation
				if(!mob.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid)) {
					startRetreatTime = Time.time;
					mob.NavAgent.isStopped = false;
				}
            }

			public override State OnUpdate(Mob mob) {
				// If the search fails to find a retreat path, immediately go to idle
				if(mob.NavAgent.isStopped) return FollowMachine.IdleState;

				// If the retreat path traversal has been completed, go to idle
				if(mob.NavAgent.remainingDistance < mob.NavAgent.stoppingDistance) return FollowMachine.IdleState;

				// If retreat timer has been exceeded, go to idle
				if(Time.time > startRetreatTime + maxRetreatDuration) return FollowMachine.IdleState;

				return this;
			}

            public override void OnExit(Mob mob) {
				mob.NavAgent.isStopped = true;
            }
        }
	}
}
