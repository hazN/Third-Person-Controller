using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.StateMachine
{
    public class EnemyDeadState : EnemyBaseState
    {
        public EnemyDeadState(StateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            enemyStateMachine.Ragdoll.ToggleRagdoll(true);
            enemyStateMachine.Weapon.gameObject.SetActive(false);
            GameObject.Destroy(enemyStateMachine.Target);
        }

        public override void Exit()
        {
        }

        public override void Tick(float deltaTime)
        {
        }
    }
}