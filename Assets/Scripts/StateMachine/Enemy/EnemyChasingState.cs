using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.StateMachine
{
    public class EnemyChasingState : EnemyBaseState
    {
        private const float DampTime = 0.1f;
        private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        private readonly int SpeedHash = Animator.StringToHash("Speed");
        public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }
        public override void Enter()
        {
            enemyStateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, FixedTransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            if (!IsPlayerInRange())
            {
                enemyStateMachine.SwitchState(new EnemyIdleState(enemyStateMachine));
                return;
            }
            else if (IsPlayerInAttackRange())
            {
                enemyStateMachine.SwitchState(new EnemyAttackingState(enemyStateMachine, 0));
                return;
            }

            MoveToPlayer(deltaTime);

            FaceTarget(enemyStateMachine.Player.transform);

            enemyStateMachine.Animator.SetFloat(SpeedHash, 1f, DampTime, deltaTime);
        }
        public override void Exit()
        {
            enemyStateMachine.Agent.ResetPath();
            enemyStateMachine.Agent.velocity = Vector3.zero;
        }
        private void MoveToPlayer(float deltaTime)
        {
            if (enemyStateMachine.Agent.isOnNavMesh)
            {
                enemyStateMachine.Agent.SetDestination(enemyStateMachine.Player.transform.position);
                Move(enemyStateMachine.Agent.desiredVelocity.normalized * enemyStateMachine.MovementSpeed, deltaTime);
            }
            enemyStateMachine.Agent.velocity = enemyStateMachine.Controller.velocity;
        }
    }
}