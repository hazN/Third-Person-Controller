using System.Collections;
using System.Collections.Generic;
using TPC.StateMachine;
using UnityEngine;
namespace TPC.StateMachine
{
    public class PlayerRollingState : PlayerBaseState
    {
        private readonly int RollHash = Animator.StringToHash("RollBlendTree");
        private readonly int ForwardHash = Animator.StringToHash("RollForward");
        private readonly int RightHash = Animator.StringToHash("RollRight");
        public PlayerRollingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            if (playerStateMachine.Stamina.TryUseStamina(playerStateMachine.DodgeStaminaCost))
            {
                if (playerStateMachine.Targeter.CurrentTarget == null)
                {
                    dodgingDirectionInput.x = 0f;
                    dodgingDirectionInput.y = 1f;
                }
                else
                {
                    dodgingDirectionInput = playerStateMachine.InputReader.MovementValue;
                }
                remainingDodgeDuration = playerStateMachine.DodgeDuration;
                playerStateMachine.Animator.CrossFadeInFixedTime(RollHash, 0.1f);
            }
        }
        public override void Tick(float deltaTime)
        { 
            playerStateMachine.Animator.SetFloat(ForwardHash, dodgingDirectionInput.y);
            playerStateMachine.Animator.SetFloat(RightHash, dodgingDirectionInput.x);
            FaceTarget(playerStateMachine.Targeter.CurrentTarget?.transform);
            Move(CalculateMovement(deltaTime) * playerStateMachine.TargetingMovementSpeed, deltaTime);

            if (remainingDodgeDuration <= 0)
            {
                ReturnToLocomotion();
            }
        }
        public override void Exit()
        {
        }
    }
}