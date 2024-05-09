using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace TPC.StateMachine
{
    public abstract class EnemyBaseState : State
    {
        protected EnemyStateMachine enemyStateMachine;
        protected EnemyBaseState(StateMachine stateMachine) : base(stateMachine)
        {
            enemyStateMachine = stateMachine as EnemyStateMachine;
        }

        protected bool IsPlayerInRange()
        {
            if (enemyStateMachine.Player.IsDead)
                return false;

            float playerDistanceSqr = (enemyStateMachine.Player.transform.position - enemyStateMachine.transform.position).sqrMagnitude;

            return playerDistanceSqr <= enemyStateMachine.DetectionRange * enemyStateMachine.DetectionRange;
        }
        protected bool IsPlayerInAttackRange()
        { 
            return IsPlayerInAttackRange(enemyStateMachine.AttackRange);
        }
        protected bool IsPlayerInAttackRange(float range)
        {
            if (enemyStateMachine.Player.IsDead)
                return false;

            float playerDistanceSqr = (enemyStateMachine.Player.transform.position - enemyStateMachine.transform.position).sqrMagnitude;
            return playerDistanceSqr <= range * range;
        }
    }
}