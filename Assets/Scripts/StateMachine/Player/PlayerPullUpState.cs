using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerPullUpState : PlayerBaseState
    {
        private readonly int PullUpHash = Animator.StringToHash("PullUp");
        public PlayerPullUpState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            playerStateMachine.Animator.CrossFadeInFixedTime(PullUpHash, FixedTransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            if (playerStateMachine.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                playerStateMachine.Controller.enabled = false;
                playerStateMachine.transform.Translate(playerStateMachine.PullUpOffset, Space.Self);
                playerStateMachine.Controller.enabled = true;
                playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine, false));
            }
        }
        public override void Exit()
        {
            playerStateMachine.Controller.Move(Vector3.zero);
            playerStateMachine.ForceReceiver.Reset();
        }
    }
}