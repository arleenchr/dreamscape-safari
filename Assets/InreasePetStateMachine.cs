using BondomanShooter.Structs;
using UnityEngine;
using UnityEngine.AI;

namespace BondomanShooter.Entities.Pets
{
    public class IncreasePetStateMachine : StateMachine<PetController>
    {
        [Header("Properties")]
        [SerializeField] protected float startChaseRange;

        [Header("Attributes")]
        [SerializeField] protected float pathRefreshInterval;

        public State IdleState { get; private set; }
        public State TakeOffState { get; private set; }
        public State MoveState { get; private set; }
        public State HoverState { get; private set; }
        public State LandingState { get; private set; }
        public State AvoidState { get; private set; }

        private void Awake()
        {
            IdleState = new Idle(this, startChaseRange);
            TakeOffState = new TakeOff(this);
            MoveState = new Move(this, pathRefreshInterval, startChaseRange);
            HoverState = new Hover(this, startChaseRange);
            LandingState = new Landing(this);
            AvoidState = new Avoid(this, pathRefreshInterval, startChaseRange);

            CurrentState = IdleState;
        }

        public abstract class IncreasePetState : State
        {
            protected IncreasePetStateMachine IncreaseMachine => (IncreasePetStateMachine)Machine;

            public IncreasePetState(IncreasePetStateMachine stateMachine)
            {
                Machine = stateMachine;
            }
        }

        public class Idle : IncreasePetState
        {
            public float startChaseRange;

            public Idle(IncreasePetStateMachine stateMachine, float startChaseRange) : base(stateMachine)
            {
                this.startChaseRange = startChaseRange;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("idle");
                Debug.Log("IDLE STATE");

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                pet.Target = player.transform;
            }

            public override State OnUpdate(PetController pet)
            {
                float sqrDist = (pet.Owner.transform.position - pet.transform.position).sqrMagnitude;

                // If target has entered the chase range, start chasing
                if (sqrDist > startChaseRange * startChaseRange)
                {
                    return IncreaseMachine.TakeOffState;
                }

                if ((pet.transform.position - pet.Target.position).sqrMagnitude < startChaseRange * startChaseRange)
                {
                    return IncreaseMachine.AvoidState;
                }

                return this;
            }
        }

        public class TakeOff : IncreasePetState
        {
            public TakeOff(IncreasePetStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("takeoff");
                Debug.Log("TAKEOFF STATE");
            }

            public override State OnUpdate(PetController pet)
            {
                if (pet.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f)
                {
                    return IncreaseMachine.MoveState;
                }

                return this;
            }
        }

        public class Move : IncreasePetState
        {
            public float pathRefreshInterval, startChaseRange;

            private float lastRefreshTime;

            public Move(IncreasePetStateMachine stateMachine, float pathRefreshInterval, float startChaseRange) : base(stateMachine)
            {
                this.pathRefreshInterval = pathRefreshInterval;
                this.startChaseRange = startChaseRange;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("fly");
                Debug.Log("MOVE STATE");

                // Compute path to follow target
                pet.NavAgent.isStopped = false;
                pet.NavAgent.destination = pet.Owner.transform.position + (pet.Owner.transform.position - pet.Target.position).normalized * (startChaseRange - 1);
                lastRefreshTime = Time.time;
            }

            public override State OnUpdate(PetController pet)
            {

                // If the target is null, stop chasing
                if (pet.Target == null) return IncreaseMachine.IdleState;

                // If no path is found, stop chasing
                if (pet.NavAgent.pathStatus.HasFlag(UnityEngine.AI.NavMeshPathStatus.PathInvalid)) return IncreaseMachine.IdleState;

                // On every path refresh...
                if (Time.time > lastRefreshTime + pathRefreshInterval)
                {
                    lastRefreshTime = Time.time;

                    // If the target is reached, stop chasing
                    if (pet.NavAgent.remainingDistance < pet.NavAgent.stoppingDistance)
                    {
                        return IncreaseMachine.HoverState;
                    }

                    // Recompute path to follow owner
                    pet.NavAgent.destination = pet.Owner.transform.position + (pet.Owner.transform.position - pet.Target.position).normalized * (startChaseRange - 1);
                }

                return this;
            }

            public override void OnExit(PetController pet)
            {
                pet.NavAgent.isStopped = true;
            }
        }

        public class Hover : IncreasePetState
        {
            private float slipTime;
            public float startChaseRange;
            public Hover(IncreasePetStateMachine stateMachine, float startChaseRange) : base(stateMachine)
            {
                this.startChaseRange = startChaseRange;
            }

            public override void OnEnter(PetController pet)
            {
                slipTime = Time.time;
                pet.Animator.SetTrigger("stop");
                Debug.Log("HOVER STATE");
            }

            public override State OnUpdate(PetController pet)
            {
                float sqrDist = (pet.Owner.transform.position - pet.transform.position).sqrMagnitude;

                // If target has entered the chase range, start chasing
                if (sqrDist > startChaseRange * startChaseRange)
                {
                    return IncreaseMachine.MoveState;
                }

                if ((pet.transform.position - pet.Target.position).sqrMagnitude < startChaseRange * startChaseRange)
                {
                    return IncreaseMachine.AvoidState;
                }

                if (Time.time - slipTime > 5f)
                {
                    return IncreaseMachine.LandingState;
                }

                return this;
            }
        }

        public class Landing : IncreasePetState
        {
            public Landing(IncreasePetStateMachine stateMachine) : base(stateMachine) { }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("land");
                Debug.Log("LANDING STATE");
            }

            public override State OnUpdate(PetController pet)
            {
                if (pet.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .5f)
                {
                    return IncreaseMachine.IdleState;
                }

                return this;
            }
        }

        public class Avoid : IncreasePetState
        {
            public float pathRefreshInterval, startChaseRange;
            private float lastRefreshTime;

            public Avoid(IncreasePetStateMachine stateMachine, float pathRefreshInterval, float startChaseRange) : base(stateMachine) 
            { 
                this.pathRefreshInterval = pathRefreshInterval;
                this.startChaseRange = startChaseRange;

            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("fly");
                Debug.Log("AVOID STATE");

                pet.NavAgent.isStopped = false;
                pet.NavAgent.destination = pet.Owner.transform.position + (pet.Owner.transform.position - pet.Target.position).normalized * (startChaseRange - 1);
                lastRefreshTime = Time.time;
            }

            public override State OnUpdate(PetController pet)
            {
                // On every path refresh...
                if (Time.time > lastRefreshTime + pathRefreshInterval)
                {
                    lastRefreshTime = Time.time;

                    // Compute the distance from the target and the owner
                    /*float distanceToTarget = Vector3.Distance(pet.Target.position, pet.transform.position);
                    float distanceToOwner = Vector3.Distance(pet.Owner.transform.position, pet.transform.position);

                    // If the pet has reached the farthest point from the target within startChaseRange, switch to Hover state
                    if (distanceToTarget >= startChaseRange && distanceToOwner <= startChaseRange)
                    {
                        return IncreaseMachine.HoverState;
                    }*/

                    if ((pet.transform.position - pet.Target.position).sqrMagnitude > startChaseRange * startChaseRange)
                    {
                        return IncreaseMachine.HoverState;
                    }

                    // Recompute path to avoid target
                    pet.NavAgent.destination = pet.Owner.transform.position + (pet.Owner.transform.position - pet.Target.position).normalized * (startChaseRange - 1);
                }

                return this;
            }

            public override void OnExit(PetController pet)
            {
                pet.NavAgent.isStopped = true;
            }
        }
    }
}
