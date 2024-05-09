using UnityEngine;
using UnityEngine.Rendering;

namespace TPC.StateMachine
{
    public class PlayerFreeLookState : PlayerBaseState
    {
        private const float DampTime = 0.1f;
        private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
        private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
        private bool shouldFade;
        public PlayerFreeLookState(PlayerStateMachine stateMachine, bool shouldFade = true) : base(stateMachine)
        {
            this.shouldFade = shouldFade;
        }

        public override void Enter()
        {
            playerStateMachine.InputReader.TargetEvent += OnTarget;
            playerStateMachine.InputReader.DodgeEvent += OnDodge;
            playerStateMachine.InputReader.JumpEvent += OnJump;
            playerStateMachine.LedgeDetector.OnClimb += OnClimbUp;

            playerStateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, DampTime, Time.deltaTime);

            if (shouldFade)
            {
                playerStateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, FixedTransitionDuration);
            }
            else
            {
                playerStateMachine.Animator.Play(FreeLookBlendTreeHash);
            }
        }

        public override void Tick(float deltaTime)
        {
            if (playerStateMachine.InputReader.IsAttacking)
            {
                playerStateMachine.SwitchState(new PlayerAttackingState(playerStateMachine, 0));
                return;
            }

            Vector3 movement = CalculateMovementCamera();

            Move(movement * playerStateMachine.FreeLookMovementSpeed, deltaTime);

            if (playerStateMachine.InputReader.MovementValue == Vector2.zero)
            {
                playerStateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, DampTime, deltaTime);
                return;
            }

            playerStateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, DampTime, deltaTime);
            FaceMovementDirection(movement);
        }

        public override void Exit()
        {
            playerStateMachine.InputReader.TargetEvent -= OnTarget;
            playerStateMachine.InputReader.DodgeEvent -= OnDodge;
            playerStateMachine.InputReader.JumpEvent -= OnJump;
            playerStateMachine.LedgeDetector.OnClimb -= OnClimbUp;
        }

        private void FaceMovementDirection(Vector3 movement)
        {
            playerStateMachine.transform.rotation = Quaternion.Slerp(playerStateMachine.transform.rotation, Quaternion.LookRotation(movement), playerStateMachine.RotationDamping * Time.deltaTime);
        }

        private void OnTarget()
        {
            if (playerStateMachine.Targeter.SelectTarget())
            {
                playerStateMachine.SwitchState(new PlayerTargetingState(playerStateMachine));
            }
        }
    }
}