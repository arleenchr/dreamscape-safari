using UnityEngine;

namespace BondomanShooter.Structs {
    public abstract class StateMachine<T> : MonoBehaviour {
        public State CurrentState { get; protected set; }

        public virtual void OnUpdate(T target) {
            State nextState = CurrentState.OnUpdate(target);
            if(nextState != CurrentState) {
                CurrentState.OnExit(target);
                CurrentState = nextState;
                CurrentState.OnEnter(target);
            }
        }

        public abstract class State {
            public StateMachine<T> Machine { get; protected set; }

            public virtual void OnEnter(T target) { }
            public virtual State OnUpdate(T target) => this;
            public virtual void OnExit(T target) { }
        }
    }
}
