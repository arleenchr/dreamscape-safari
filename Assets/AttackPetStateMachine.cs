using BondomanShooter.Structs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BondomanShooter.Entities.Pets
{
    public class AttackPetStateMachine : StateMachine<PetController>
    {
        [Header("Properties")]
        [SerializeField] protected float startChaseRange;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float retreatDistance;

        [Header("Attributes")]
        [SerializeField] protected float pathRefreshInterval;

        public State IdleState { get; private set; }
        public State TakeOffState { get; private set; }
        public State MoveState { get; private set; }
        public State HoverState { get; private set; }
        public State LandingState { get; private set; }
        public State AttackState { get; private set; }
        public State RetreatState { get; private set; }

        private void Awake()
        {
            IdleState = new Idle(this, startChaseRange, attackRange);
            TakeOffState = new TakeOff(this);
            MoveState = new Move(this, pathRefreshInterval, attackRange);
            HoverState = new Hover(this, startChaseRange, attackRange);
            LandingState = new Landing(this);
            AttackState = new Attack(this, retreatDistance);
            RetreatState = new Retreat(this, pathRefreshInterval);
            
            CurrentState = IdleState;
        }

        private class MobDistanceComparer : IComparer<GameObject>
        {
            private Vector3 from;

            public MobDistanceComparer(Vector3 from)
            {
                this.from = from;
            }

            public int Compare(GameObject a, GameObject b)
            {
                float distanceToA = Vector3.Distance(from, a.transform.position);
                float distanceToB = Vector3.Distance(from, b.transform.position);

                return distanceToA.CompareTo(distanceToB);
            }
        }

        public abstract class AttackPetState : State
        {
            protected AttackPetStateMachine FollowMachine => (AttackPetStateMachine)Machine;

            public AttackPetState(AttackPetStateMachine stateMachine)
            {
                Machine = stateMachine;
            }
        }

        public class Idle : AttackPetState
        {
            public float startChaseRange;
            public float attackRange;

            public Idle(AttackPetStateMachine stateMachine, float startChaseRange, float attackRange) : base(stateMachine)
            {
                this.startChaseRange = startChaseRange;
                this.attackRange = attackRange;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("idle");
            }

            public override State OnUpdate(PetController pet)
            {
                // Do nothing if there is no target
                if (pet.Target == null) return this;
                
                GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
                Array.Sort(mobs, new MobDistanceComparer(pet.transform.position));


                if (mobs.Length > 0 && (mobs[0].transform.position - pet.transform.position).sqrMagnitude < attackRange * attackRange)
                {
                    pet.Target = mobs[0].transform;
                    // Debug.Log(pet.Target);
                }
                else
                {
                    pet.Target = pet.Owner.transform;
                }

                float sqrDist = (pet.Target.position - pet.transform.position).sqrMagnitude;

                // If target has entered the chase range, start chasing
                if (sqrDist > startChaseRange * startChaseRange)
                {
                    return FollowMachine.TakeOffState;
                }

                return this;
            }
        }

        public class TakeOff : AttackPetState
        {
            public TakeOff(AttackPetStateMachine stateMachine) : base(stateMachine) { }

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

        public class Move : AttackPetState
        {
            public float attackArc, pathRefreshInterval, attackRange;

            private float lastRefreshTime;

            public Move(AttackPetStateMachine stateMachine, float pathRefreshInterval, float attackRange) : base(stateMachine)
            {
                this.pathRefreshInterval = pathRefreshInterval;
                this.attackRange = attackRange;
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


                GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
                Array.Sort(mobs, new MobDistanceComparer(pet.transform.position));

                if (mobs.Length > 0 && (mobs[0].transform.position - pet.transform.position).sqrMagnitude < attackRange * attackRange)
                {
                    pet.Target = mobs[0].transform;
                    // Debug.Log(pet.Target);
                }
                else
                {
                    pet.Target = pet.Owner.transform;
                }

                // On every path refresh...
                if (Time.time > lastRefreshTime + pathRefreshInterval)
                {
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

        public class Hover : AttackPetState
        {
            private float slipTime;
            public float startChaseRange, attackRange;
            public Hover(AttackPetStateMachine stateMachine, float startChaseRange, float attackRange) : base(stateMachine)
            {
                this.startChaseRange = startChaseRange;
                this.attackRange = attackRange;
            }

            public override void OnEnter(PetController pet)
            {
                slipTime = Time.time;
                pet.Animator.SetTrigger("stop");
            }

            public override State OnUpdate(PetController pet)
            {
                GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
                Array.Sort(mobs, new MobDistanceComparer(pet.transform.position));

                if (mobs.Length > 0 && (mobs[0].transform.position - pet.transform.position).sqrMagnitude < attackRange * attackRange)
                {
                    pet.Target = mobs[0].transform;
                    // Debug.Log(pet.Target);
                }
                else
                {
                    pet.Target = pet.Owner.transform;
                }

                float sqrDist = (pet.Target.position - pet.transform.position).sqrMagnitude;

                // If the distance to the target reaches the attack range, check attack arc
                if (pet.Target != pet.Owner.transform && sqrDist < attackRange * attackRange)
                {
                    return FollowMachine.AttackState;
                    /* Vector2 dir2 = new Vector2(dir.x, dir.z).normalized;
                     Vector2 fwd2 = new(pet.transform.forward.x, pet.transform.forward.z);

                     // If target is within the attack arc, begin attacking
                     if (Vector2.Angle(dir2, fwd2) < attackArc / 2f)
                     {
                         return FollowMachine.AttackState;
                     }*/
                }

                if (pet.Target == mobs[0].transform)
                {
                    return FollowMachine.MoveState;
                }

                if (pet.Target == pet.Owner.transform && sqrDist > startChaseRange * startChaseRange)
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

        public class Landing : AttackPetState
        {
            public Landing(AttackPetStateMachine stateMachine) : base(stateMachine) { }

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

        public class Attack : AttackPetState
        {
            public float retreatDistance;

            public Attack(AttackPetStateMachine stateMachine, float retreatDistance) : base(stateMachine) 
            {
                this.retreatDistance = retreatDistance;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("attack");
                pet.WeaponOwner.PrimaryAction(true);
            }

            public override State OnUpdate(PetController pet)
            {
                float sqrDist = (pet.Owner.transform.position - pet.transform.position).sqrMagnitude;

                if (sqrDist > retreatDistance * retreatDistance)
                {
                    pet.Target = pet.Owner.transform;
                    
                    if (pet.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        return (FollowMachine.RetreatState);
                    }
                }

                /*if (pet.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    return FollowMachine.HoverState;
                }*/

                return this;
            }

            public override void OnExit(PetController pet)
            {
                pet.WeaponOwner.PrimaryAction(false);
            }
        }

        public class Retreat : AttackPetState
        {
            public float pathRefreshInterval;
            private float lastRefreshTime;

            public Retreat(AttackPetStateMachine stateMachine, float pathRefreshInterval) : base(stateMachine)
            {
                this.pathRefreshInterval = pathRefreshInterval;
            }

            public override void OnEnter(PetController pet)
            {
                pet.Animator.SetTrigger("fly");

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
    }
}