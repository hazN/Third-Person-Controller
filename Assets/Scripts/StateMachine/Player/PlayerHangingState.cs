using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerHangingState : PlayerBaseState
    {
        private readonly int HangingHash = Animator.StringToHash("Hanging");
        private Vector3 ledgeForward;
        public PlayerHangingState(PlayerStateMachine stateMachine, Vector3 ledgeForward) : base(stateMachine)
        {
            this.ledgeForward = ledgeForward;
        }

        public override void Enter()
        {
            playerStateMachine.transform.rotation = Quaternion.LookRotation(ledgeForward, Vector3.up);

            playerStateMachine.Animator.CrossFadeInFixedTime(HangingHash, FixedTransitionDuration);
        }
        public override void Tick(float deltaTime)
        {
            if (playerStateMachine.InputReader.MovementValue.y < 0f)
            {
                playerStateMachine.ForceReceiver.Reset();
                playerStateMachine.SwitchState(new PlayerFallingState(playerStateMachine));
            }
            else if (playerStateMachine.InputReader.MovementValue.y > 0f)
            {
                playerStateMachine.SwitchState(new PlayerPullUpState(playerStateMachine));
            }
        }
        public override void Exit()
        {
        }
    }
}