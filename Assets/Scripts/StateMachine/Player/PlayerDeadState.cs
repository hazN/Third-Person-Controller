using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerDeadState : PlayerBaseState
    {
        public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            playerStateMachine.Ragdoll.ToggleRagdoll(true);
            playerStateMachine.Weapon.gameObject.SetActive(false);
        }

        public override void Exit()
        { }

        public override void Tick(float deltaTime)
        { }
    }
}