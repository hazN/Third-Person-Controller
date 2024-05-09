using System;
using TPC.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace TPC.StateMachine
{
    public class PlayerTargetingState : PlayerBaseState
    {
        private readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");
        private readonly int TargetingForwardHash = Animator.StringToHash("TargetingForward");
        private readonly int TargetingRightHash = Animator.StringToHash("TargetingRight");
        public PlayerTargetingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
        {
        }

        public override void Enter()
        {
            playerStateMachine.InputReader.CancelEvent += OnCancel;
            playerStateMachine.InputReader.TargetEvent += OnTarget;
            playerStateMachine.InputReader.GrabEvent += OnGrab;
            playerStateMachine.InputReader.DodgeEvent += OnDodge;
            playerStateMachine.InputReader.JumpEvent += OnJump;
            playerStateMachine.Animator.CrossFadeInFixedTime(TargetingBlendTreeHash, FixedTransitionDuration);
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void Tick(float deltaTime)
        {
            if (playerStateMachine.InputReader.IsAttacking)
            {
                playerStateMachine.SwitchState(new PlayerAttackingState(playerStateMachine, 0));
                return;
            }
            if (playerStateMachine.InputReader.IsBlocking && playerStateMachine.Stamina.CanUseStamina(playerStateMachine.BlockStaminaCost * 100f))
            {
                playerStateMachine.SwitchState(new PlayerBlockingState(playerStateMachine));
                return;
            }
            if (playerStateMachine.Targeter.CurrentTarget == null)
            {
                playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine));
                return;
            }
            UpdateAnimator(deltaTime);
            FaceTarget(playerStateMachine.Targeter.CurrentTarget.transform);
            Move(CalculateMovement(deltaTime) * playerStateMachine.TargetingMovementSpeed, deltaTime);
        }

        public override void Exit()
        {
            playerStateMachine.InputReader.CancelEvent -= OnCancel;
            playerStateMachine.InputReader.TargetEvent -= OnTarget;
            playerStateMachine.InputReader.GrabEvent -= OnGrab;
            playerStateMachine.InputReader.DodgeEvent -= OnDodge;
            playerStateMachine.InputReader.JumpEvent -= OnJump;
        }

        private void OnCancel()
        {
            playerStateMachine.Targeter.Cancel();
            playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine));
        }
        private void UpdateAnimator(float dt)
        {
            playerStateMachine.Animator.SetFloat(TargetingForwardHash, playerStateMachine.InputReader.MovementValue.y, 0.1f, dt);
            playerStateMachine.Animator.SetFloat(TargetingRightHash, playerStateMachine.InputReader.MovementValue.x, 0.1f, dt);
        }
        private void OnTarget()
        {
            if (playerStateMachine.Targeter.SelectTarget())
            {
                FaceTarget(playerStateMachine.Targeter.CurrentTarget.transform);
            }
        }
        private void OnGrab()
        {
        }
    }
}