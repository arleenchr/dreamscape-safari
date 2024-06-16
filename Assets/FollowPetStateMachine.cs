using BondomanShooter.Entities.Mobs;
using BondomanShooter.Structs;
using System.Reflection;
using UnityEngine;

namespace BondomanShooter.Entities.Pets
{
    public class FollowPetStateMachine : StateMachine<PetController>
    {
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
        public State TakeOffState { get; private set; }
        public State MoveState { get; private set; }
        public State HoverState { get; private set; }
        public State LandingState { get; private set; }

        private void Awake()
        {
            IdleState = new Idle(this, startChaseRange);
            TakeOffState = new TakeOff(this);
            MoveState = new Move(this, stopChaseRange, attackRange, attackArc, pathRefreshInterval);
            HoverState = new Hover(this, startChaseRange);
            LandingState = new Landing(this);

            CurrentState = IdleState;
        }

        public abstract class FollowPetState : State
        {
            protected FollowPetStateMachine FollowMachine => (FollowPetStateMachine)Machine;

            public FollowPetState(FollowPetStateMachine stateMachine)
            {
                Machine = stateMachine;
            }
        }

        public class Idle : FollowPetState
        {
            public float startChaseRange;

            public Idle(FollowPetStateMachine stateMachine, float startChaseRange) : base(stateMachine)
            {
                this.startChaseRange = startChaseRange;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("idle");
            }

            public override State OnUpdate(PetController pet)
            {
                // Do nothing if there is no target
                if (pet.Target == null) return this;

                float sqrDist = (pet.Target.position - pet.transform.position).sqrMagnitude;

                // If target has entered the chase range, start chasing
                if (sqrDist > startChaseRange * startChaseRange)
                {
                    return FollowMachine.TakeOffState;
                }

                return this;
            }
        }

        public class TakeOff : FollowPetState
        {
            public TakeOff(FollowPetStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("takeoff");
            }

            public override State OnUpdate(PetController pet)
            {
                if (pet.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f)
                {
                    return FollowMachine.MoveState;
                }

                return this;
            }
        }

        public class Move : FollowPetState
        {
            public float stopChaseRange, attackRange, attackArc, pathRefreshInterval;

            private float lastRefreshTime;

            public Move(FollowPetStateMachine stateMachine, float stopChaseRange, float attackRange, float attackArc, float pathRefreshInterval) : base(stateMachine)
            {
                this.stopChaseRange = stopChaseRange;
                this.attackRange = attackRange;
                this.attackArc = attackArc;
                this.pathRefreshInterval = pathRefreshInterval;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("fly");

                // Compute path to follow target
                pet.NavAgent.isStopped = false;
                pet.NavAgent.destination = pet.Target.position;
                lastRefreshTime = Time.time;
            }

            public override State OnUpdate(PetController pet)
            {

                // If the target is null, stop chasing
                if (pet.Target == null) return FollowMachine.IdleState;

                // If no path is found, stop chasing
                if (pet.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid)) return FollowMachine.IdleState;

                // On every path refresh...
                if (Time.time > lastRefreshTime + pathRefreshInterval)
                {
                    lastRefreshTime = Time.time;

                    // If the target is reached, stop chasing
                    if (pet.NavAgent.remainingDistance < pet.NavAgent.stoppingDistance)
                    {
                        return FollowMachine.HoverState;
                    }

                    // Recompute path to follow target
                    pet.NavAgent.destination = pet.Target.position;
                }

                return this;
            }

            public override void OnExit(PetController pet)
            {
                pet.NavAgent.isStopped = true;
            }
        }

        public class Hover : FollowPetState
        {
            private float slipTime;
            public float startChaseRange;
            public Hover(FollowPetStateMachine stateMachine, float startChaseRange) : base(stateMachine)
            {
                this.startChaseRange = startChaseRange;
            }

            public override void OnEnter(PetController pet)
            {
                slipTime = Time.time;
                pet.Animator.SetTrigger("stop");
            }

            public override State OnUpdate(PetController pet)
            {
                float sqrDist = (pet.Target.position - pet.transform.position).sqrMagnitude;

                if (sqrDist > startChaseRange * startChaseRange)
                {
                    return FollowMachine.MoveState;
                }
                
                if (Time.time - slipTime > 5f)
                {
                    return FollowMachine.LandingState;
                }
                
                return this;
            }
        }

        public class Landing : FollowPetState
        {
            public Landing(FollowPetStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("land");
            }

            public override State OnUpdate(PetController pet)
            {
                if (pet.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f)
                {
                    return FollowMachine.IdleState;
                }

                return this;
            }
        }
    }
}
