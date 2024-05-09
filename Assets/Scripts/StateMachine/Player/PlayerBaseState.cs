using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.StateMachine
{
    public abstract class PlayerBaseState : State
    {
        public PlayerStateMachine playerStateMachine;
        protected float remainingDodgeDuration;
        protected Vector2 dodgingDirectionInput;

        protected PlayerBaseState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
            playerStateMachine = stateMachine as PlayerStateMachine;
        }
        protected void ReturnToLocomotion()
        {
            if (playerStateMachine.Targeter.CurrentTarget != null)
            {
                playerStateMachine.SwitchState(new PlayerTargetingState(playerStateMachine));
            }
            else
            {
                playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine));
            }
        }
        protected Vector3 CalculateMovement(float dt)
        {
            Vector3 movement = new Vector3();
            if (remainingDodgeDuration > 0)
            {
                movement += playerStateMachine.transform.forward * dodgingDirectionInput.y * playerStateMachine.DodgeLength / playerStateMachine.DodgeDuration;
                movement += playerStateMachine.transform.right * dodgingDirectionInput.x * playerStateMachine.DodgeLength / playerStateMachine.DodgeDuration;
                remainingDodgeDuration -= dt;

                remainingDodgeDuration = Mathf.Max(remainingDodgeDuration, 0);

                return movement;
            }
            {
                movement += playerStateMachine.transform.right * playerStateMachine.InputReader.MovementValue.x;
                movement += playerStateMachine.transform.forward * playerStateMachine.InputReader.MovementValue.y;

                return movement;
            }
        }
        protected Vector3 CalculateMovementCamera()
        {
            Vector3 forward = playerStateMachine.MainCameraTransform.forward;
            Vector3 right = playerStateMachine.MainCameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            return forward * playerStateMachine.InputReader.MovementValue.y + right * playerStateMachine.InputReader.MovementValue.x;
        }
        protected void OnDodge()
        {
            if (playerStateMachine.Stamina.CanUseStamina(playerStateMachine.DodgeStaminaCost))
            {
                playerStateMachine.SwitchState(new PlayerRollingState(playerStateMachine));
            }
        }
        protected void OnJump()
        {
            playerStateMachine.SwitchState(new PlayerJumpingState(playerStateMachine));
        }
        protected void OnClimbUp()
        {
            RaycastHit hit;
            if (playerStateMachine.LedgeDetector.TryClimb(out hit))
            {
                playerStateMachine.SwitchState(new PlayerClimbingState(playerStateMachine, hit));
            }
        }
    }
}