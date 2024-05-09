using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPC.Combat;
using Unity.VisualScripting;

namespace TPC.StateMachine
{
    public class PlayerAttackingState : PlayerBaseState
    {
        private Attack attack;
        private float previousAttackTime;
        private bool alreadyAppliedForce = false;
        public PlayerAttackingState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
        {
            attack = playerStateMachine.Attacks[attackIndex];
        }

        public override void Enter()
        {
            playerStateMachine.Weapon.SetAttack(attack.Damage, attack.Knockback);
            playerStateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            FaceTarget(playerStateMachine.Targeter.CurrentTarget?.transform);

            float normalizedTime = GetNormalizedTime();

            if (normalizedTime >= previousAttackTime && normalizedTime < 1)
            {
                if (normalizedTime >= attack.ForceTime)
                {
                    TryApplyForce(normalizedTime);
                }

                if (playerStateMachine.InputReader.IsAttacking)
                {
                    TryComboAttack(normalizedTime);
                }
            }
            else
            {
                ReturnToLocomotion();
            }

            previousAttackTime = normalizedTime;
        }
        public override void Exit()
        {
        }
        private float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = playerStateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = playerStateMachine.Animator.GetNextAnimatorStateInfo(0);
            if (playerStateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("Attack"))
            {
                return nextInfo.normalizedTime;
            }
            else if (!playerStateMachine.Animator.IsInTransition(0) && currentInfo.IsTag("Attack"))
            {
                return currentInfo.normalizedTime;
            }
            return 0;
        }
        private void TryComboAttack(float normalizedTime)
        {
            if (attack.ComboStateIndex == -1) return;

            if (normalizedTime < attack.ComboAttackTime) return;

            playerStateMachine.SwitchState(new PlayerAttackingState(playerStateMachine, attack.ComboStateIndex));
        }
        private void TryApplyForce(float normalizedTime)
        {
            if (alreadyAppliedForce) return;

            Vector3 force = playerStateMachine.transform.forward * attack.Force;
            playerStateMachine.ForceReceiver.AddForce(force);

            alreadyAppliedForce = true;
        }
    }
}