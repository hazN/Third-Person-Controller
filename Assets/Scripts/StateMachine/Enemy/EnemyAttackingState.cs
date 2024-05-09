using TPC.Combat;
using UnityEngine;

namespace TPC.StateMachine
{
    public class EnemyAttackingState : EnemyBaseState
    {
        private Attack attack;
        private float previousAttackTime;
        private bool alreadyAppliedForce = false;

        public EnemyAttackingState(StateMachine stateMachine, int attackIndex) : base(stateMachine)
        {
            attack = enemyStateMachine.Attacks[attackIndex];
        }

        public override void Enter()
        {
            enemyStateMachine.Weapon.SetAttack(attack.Damage, attack.Knockback);
            enemyStateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
            FaceTarget(enemyStateMachine.Player.transform);
        }

        public override void Tick(float deltaTime)
        {
            Move(deltaTime);

            float normalizedTime = GetNormalizedTime();

            if (normalizedTime >= previousAttackTime && normalizedTime < 1)
            {
                if (normalizedTime >= attack.ForceTime)
                {
                    TryApplyForce(normalizedTime);
                }

                if (IsPlayerInAttackRange(enemyStateMachine.AttackRange * 1.5f) &&
                    Vector3.Dot(enemyStateMachine.transform.forward, enemyStateMachine.Player.transform.position - enemyStateMachine.transform.position) > 0)
                {
                    TryComboAttack(normalizedTime);
                }
            }
            else
            {
                enemyStateMachine.SwitchState(new EnemyIdleState(enemyStateMachine));
            }

            previousAttackTime = normalizedTime;
        }

        public override void Exit()
        {
        }

        private float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = enemyStateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = enemyStateMachine.Animator.GetNextAnimatorStateInfo(0);
            if (enemyStateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("Attack"))
            {
                return nextInfo.normalizedTime;
            }
            else if (!enemyStateMachine.Animator.IsInTransition(0) && currentInfo.IsTag("Attack"))
            {
                return currentInfo.normalizedTime;
            }
            return 0;
        }

        private void TryComboAttack(float normalizedTime)
        {
            if (attack.ComboStateIndex == -1) return;

            if (normalizedTime < attack.ComboAttackTime) return;

            enemyStateMachine.SwitchState(new EnemyAttackingState(enemyStateMachine, attack.ComboStateIndex));
        }

        private void TryApplyForce(float normalizedTime)
        {
            if (alreadyAppliedForce) return;

            Vector3 force = enemyStateMachine.transform.forward * attack.Force;
            enemyStateMachine.ForceReceiver.AddForce(force);

            alreadyAppliedForce = true;
        }
    }
}