using UnityEngine;
namespace TPC.StateMachine
{
    public abstract class State
    {
        protected StateMachine stateMachine;
        protected const float FixedTransitionDuration = 0.2f;

        protected State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        public abstract void Enter();
        public abstract void Tick(float deltaTime);
        public abstract void Exit();

        protected void Move(float dt)
        {
            Move(Vector3.zero, dt);
        }
        protected void Move(Vector3 motion, float dt)
        {
            stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * dt);
        }
        protected virtual void FaceTarget(Transform target)
        {
            if (target == null) return;
            Vector3 lookDirection = target.position - stateMachine.transform.position;
            lookDirection.y = 0;
            stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, Quaternion.LookRotation(lookDirection), stateMachine.RotationDamping * Time.deltaTime);
        }
    }
}