using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace TPC.StateMachine
{
    public class EnemyIdleState : EnemyBaseState
    {
        private const float DampTime = 0.1f;
        private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        private readonly int SpeedHash = Animator.StringToHash("Speed");
        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        public override void Enter()
        {
            enemyStateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, FixedTransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            if (IsPlayerInRange())
            {
                enemyStateMachine.SwitchState(new EnemyChasingState(enemyStateMachine));
                return;
            }
            enemyStateMachine.Animator.SetFloat(SpeedHash, 0f, DampTime, deltaTime);
        }
        public override void Exit()
        {
        }
    }
}