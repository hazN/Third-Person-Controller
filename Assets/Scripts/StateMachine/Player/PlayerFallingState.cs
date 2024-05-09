using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerFallingState : PlayerBaseState
    {
        private readonly int FallHash = Animator.StringToHash("Fall");
        private Vector3 momentum = Vector3.zero;
        private const float MaxMomentum = 5f;
        public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            momentum = playerStateMachine.Controller.velocity;
            momentum.y = 0;

            playerStateMachine.Animator.CrossFadeInFixedTime(FallHash, FixedTransitionDuration);

            playerStateMachine.LedgeDetector.OnClimb += OnClimbUp;
        }
        public override void Tick(float deltaTime)
        {
            if (playerStateMachine.Controller.isGrounded)
            {
                ReturnToLocomotion();
                return;
            }

            if (!playerStateMachine.Targeter.HasTarget())
            {
                momentum += CalculateMovementCamera().normalized;
                playerStateMachine.transform.rotation = Quaternion.Slerp(playerStateMachine.transform.rotation, Quaternion.LookRotation(momentum), playerStateMachine.RotationDamping * Time.deltaTime);
            }

            momentum = Vector3.ClampMagnitude(momentum, 5f);
            Move(momentum, deltaTime);

            FaceTarget(playerStateMachine.Targeter.CurrentTarget?.transform);
        }
        public override void Exit()
        {
            playerStateMachine.LedgeDetector.OnClimb -= OnClimbUp;
        }
        private void HandleLedgeDetect(Vector3 ledgeForward)
        {
            playerStateMachine.SwitchState(new PlayerHangingState(playerStateMachine, ledgeForward));
        }
    }
}