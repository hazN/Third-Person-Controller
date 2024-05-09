using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private readonly int JumpHash = Animator.StringToHash("Jump");
        private Vector3 momentum = Vector3.zero;
        private float previousTickVelocity = 0;
        public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            playerStateMachine.ForceReceiver.Jump(playerStateMachine.JumpForce);

            momentum = playerStateMachine.Controller.velocity;
            momentum.y = 0;

            playerStateMachine.Animator.CrossFadeInFixedTime(JumpHash, FixedTransitionDuration);

            playerStateMachine.LedgeDetector.OnClimb += OnClimbUp;

        }
        public override void Tick(float deltaTime)
        {
            if (!playerStateMachine.Targeter.CurrentTarget)
                momentum += CalculateMovementCamera().normalized * playerStateMachine.JumpHorizontalForce;
            else momentum += CalculateMovement(deltaTime).normalized * playerStateMachine.JumpHorizontalForce;
            if (!playerStateMachine.Targeter.CurrentTarget)
                playerStateMachine.transform.rotation = Quaternion.LookRotation(momentum.normalized, Vector3.up);
            Move(momentum, deltaTime);

            if (playerStateMachine.Controller.velocity.y < previousTickVelocity)
            {
                playerStateMachine.SwitchState(new PlayerFallingState(playerStateMachine));
                return;
            }

            previousTickVelocity = playerStateMachine.Controller.velocity.y;
        }
        public override void Exit()
        {
            playerStateMachine.LedgeDetector.OnClimb -= OnClimbUp;
        }

        private void HandleLedgeDetect(Vector3 forward)
        {
            playerStateMachine.SwitchState(new PlayerHangingState(playerStateMachine, forward));
        }
    }
}