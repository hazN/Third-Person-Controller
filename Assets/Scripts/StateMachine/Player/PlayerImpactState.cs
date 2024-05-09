using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerImpactState : PlayerBaseState
    {
        private readonly int ImpactHash = Animator.StringToHash("Impact");
        private float duration = 1f;
        private Transform damageSource;
        public PlayerImpactState(PlayerStateMachine stateMachine, Transform damageSource) : base(stateMachine)
        {
            this.damageSource = damageSource;
        }

        public override void Enter()
        {
            playerStateMachine.Animator.CrossFadeInFixedTime(ImpactHash, FixedTransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            Move(deltaTime);
            FaceTarget(damageSource);

            duration -= deltaTime;

            if (duration <= 0)
            {
                ReturnToLocomotion();
            }
        }
        public override void Exit()
        {
        }

    }
}