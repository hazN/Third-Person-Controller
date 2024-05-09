using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace TPC.StateMachine
{
    public class PlayerBlockingState : PlayerBaseState
    {
        private readonly int blockHash = Animator.StringToHash("BlockingBlendTree");
        private readonly int blockSpeedForward = Animator.StringToHash("BlockSpeedForward");
        private readonly int blockSpeedRight = Animator.StringToHash("BlockSpeedRight");
        public PlayerBlockingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            playerStateMachine.Animator.CrossFadeInFixedTime(blockHash, 0.05f);
            playerStateMachine.Health.SetBlocking(true);
        }
        public override void Tick(float deltaTime)
        {
            playerStateMachine.Animator.SetFloat(blockSpeedForward, playerStateMachine.InputReader.MovementValue.y, FixedTransitionDuration, deltaTime);
            playerStateMachine.Animator.SetFloat(blockSpeedRight, playerStateMachine.InputReader.MovementValue.x, FixedTransitionDuration, deltaTime);

            FaceTarget(playerStateMachine.Targeter.CurrentTarget?.transform);
            Move(CalculateMovement(deltaTime) * playerStateMachine.BlockingWalkSpeed, deltaTime);

            if (!playerStateMachine.InputReader.IsBlocking)
            {
                ReturnToLocomotion();
            }
            if (!playerStateMachine.Stamina.TryUseStamina(playerStateMachine.BlockStaminaCost))
            {
                ReturnToLocomotion();
            }

        }
        public override void Exit()
        {
            playerStateMachine.Health.SetBlocking(false);
        }
    }
}