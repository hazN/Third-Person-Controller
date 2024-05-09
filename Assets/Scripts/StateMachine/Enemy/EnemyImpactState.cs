using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace TPC.StateMachine
{
    public class EnemyImpactState : EnemyBaseState
    {
        private readonly int ImpactHash = Animator.StringToHash("Impact");
        private float duration = 1f;
        private Transform damageSource;
        public EnemyImpactState(StateMachine stateMachine, Transform damageSource) : base(stateMachine)
        {
            this.damageSource = damageSource;
        }
        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(ImpactHash, FixedTransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            Move(deltaTime);
            FaceTarget(damageSource);

            duration -= deltaTime;

            if (duration <= 0)
            {
                enemyStateMachine.SwitchState(new EnemyIdleState(enemyStateMachine));
            }
        }
        public override void Exit()
        {
        }
    }
}   