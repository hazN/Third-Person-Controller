using UnityEngine;

namespace TPC.StateMachine
{
    public class PlayerClimbingState : PlayerBaseState
    {
        private const float StartTime = 0.5f;
        private readonly int climbingHash = Animator.StringToHash("Climbing");

        private bool isLerping;
        private bool isMidClimb;
        private bool inPosition;
        private float posT;
        private float posX, posY;
        private Vector3 startPos, targetPos;
        private RaycastHit hit;

        public PlayerClimbingState(PlayerStateMachine stateMachine, RaycastHit hit) : base(stateMachine)
        {
            this.hit = hit;
        }

        public override void Enter()
        {
            playerStateMachine.Animator.CrossFadeInFixedTime(climbingHash, FixedTransitionDuration);
            playerStateMachine.ForceReceiver.enableGravity = false;
            playerStateMachine.Controller.enabled = false;
            playerStateMachine.EnableTools(false);

            playerStateMachine.InputReader.JumpEvent += HandleJump;
            InitForClimb(hit);
        }

        public override void Tick(float deltaTime)
        {
            Debug.DrawRay(playerStateMachine.transform.position - playerStateMachine.transform.forward + new Vector3(0f,1f,0f), Vector3.down * 0.5f, Color.red);

            if (!playerStateMachine.Stamina.CanUseStamina(playerStateMachine.LedgeDetector.staminaCost * deltaTime))
            {
                playerStateMachine.SwitchState(new PlayerFallingState(playerStateMachine));
                return;
            }

            if (!inPosition)
            {
                GetInPosition(deltaTime);
                return;
            }

            if (!isLerping)
            {
                posX = playerStateMachine.InputReader.MovementValue.x;
                posY = playerStateMachine.InputReader.MovementValue.y;

                float m = Mathf.Abs(posX) + Mathf.Abs(posY);

                Vector3 horizontal = playerStateMachine.LedgeDetector.helper.right * posX;
                Vector3 vertical = playerStateMachine.LedgeDetector.helper.up * posY;

                Vector3 direction = (horizontal + vertical).normalized;

                if (isMidClimb)
                {
                    if (direction == Vector3.zero)
                    {
                        return;
                    }
                }
                else
                {
                    bool canMove = CanMove(direction);
                    if (!canMove || direction == Vector3.zero)
                    {
                        return;
                    }
                }

                isMidClimb = !isMidClimb;

                posT = 0;
                isLerping = true;
                startPos = playerStateMachine.transform.position;

                Vector3 tempTargetPos = playerStateMachine.LedgeDetector.helper.position - playerStateMachine.transform.position;
                float halfDistance = Vector3.Distance(playerStateMachine.LedgeDetector.helper.position, startPos) / 2;
                tempTargetPos *= playerStateMachine.LedgeDetector.positionOffset;
                tempTargetPos += playerStateMachine.transform.position;
                targetPos = (isMidClimb) ? tempTargetPos : playerStateMachine.LedgeDetector.helper.position;
                playerStateMachine.LedgeDetector.climbingAnimator.CreatePositions(targetPos, direction, isMidClimb);
            }
            else
            {
                posT += deltaTime * playerStateMachine.LedgeDetector.climbSpeed;
                if (posT > 1)
                {
                    posT = 1;
                    isLerping = false;
                }

                Vector3 newTransform = Vector3.Lerp(startPos, targetPos, posT);
                playerStateMachine.transform.position = newTransform;
                playerStateMachine.transform.rotation = Quaternion.Slerp(playerStateMachine.transform.rotation,
                    playerStateMachine.LedgeDetector.helper.rotation, deltaTime * playerStateMachine.LedgeDetector.rotateSpeed);

                playerStateMachine.Stamina.TryUseStamina(playerStateMachine.LedgeDetector.staminaCost * deltaTime);
            }

            if (IsGroundedOrOutOfWall())
            {
                playerStateMachine.SwitchState(new PlayerFallingState(playerStateMachine));
                return;
            }
        }

        public override void Exit()
        {
            playerStateMachine.ForceReceiver.enableGravity = true;
            playerStateMachine.ForceReceiver.Reset();
            playerStateMachine.Controller.enabled = true;
            playerStateMachine.LedgeDetector.SetClimbing(false);
            playerStateMachine.EnableTools(true);

            playerStateMachine.InputReader.JumpEvent -= HandleJump;
        }

        private void HandleJump()
        {
            playerStateMachine.SwitchState(new PlayerFallingState(playerStateMachine));
        }

        private void GetInPosition(float deltaTime)
        {
            posT += deltaTime;

            if (posT > StartTime)
            {
                posT = StartTime;
                inPosition = true;

                playerStateMachine.LedgeDetector.climbingAnimator.CreatePositions(targetPos, Vector3.zero, false);
            }

            Vector3 newTransform = Vector3.Lerp(startPos, targetPos, posT);
            playerStateMachine.transform.position = newTransform;
            playerStateMachine.transform.rotation = Quaternion.Slerp(playerStateMachine.transform.rotation,
                   playerStateMachine.LedgeDetector.helper.rotation, deltaTime * playerStateMachine.LedgeDetector.rotateSpeed);
        }

        private Vector3 OffsetPosition(Vector3 start, Vector3 target)
        {
            Vector3 direction = start - target;
            direction.Normalize();
            Vector3 offset = direction * playerStateMachine.LedgeDetector.offsetFromWall;
            return target + offset;
        }

        private void InitForClimb(RaycastHit hit)
        {
            // Find the starting wall position to put the player at
            playerStateMachine.LedgeDetector.SetClimbing(true);
            playerStateMachine.LedgeDetector.helper.position = OffsetPosition(playerStateMachine.transform.position, hit.point);
            startPos = playerStateMachine.transform.position;
            targetPos = hit.point + (hit.normal * playerStateMachine.LedgeDetector.offsetFromWall);
            targetPos.y -= 1.5f;
            posT = 0;
            inPosition = false;

            playerStateMachine.transform.position = playerStateMachine.LedgeDetector.helper.position;
            playerStateMachine.LedgeDetector.climbingAnimator.SetStartingPosition(playerStateMachine.LedgeDetector.helper.position, Vector3.zero, false);
        }

        private bool CanMove(Vector3 direction)
        {
            Vector3 start = playerStateMachine.transform.position;
            float distance = playerStateMachine.LedgeDetector.rayToMoveDir;

            // Check for any obstacles in the way
            RaycastHit hit;
            if (Physics.Raycast(start, direction, out hit, distance, playerStateMachine.LedgeDetector.climbableLayer))
            {
                return false;
            }

            start += direction * distance;
            direction = playerStateMachine.LedgeDetector.helper.forward;
            float distance2 = playerStateMachine.LedgeDetector.rayToWall;

            // Check if there is a wall in the movement direction
            if (Physics.Raycast(start, direction, out hit, distance, playerStateMachine.LedgeDetector.climbableLayer))
            {
                playerStateMachine.LedgeDetector.helper.position = OffsetPosition(start, hit.point);
                playerStateMachine.LedgeDetector.helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

            start = start + (direction * distance2);
            if (Physics.Raycast(start, -direction, out hit, playerStateMachine.LedgeDetector.rayToWall))
            {
                playerStateMachine.LedgeDetector.helper.position = OffsetPosition(start, hit.point);
                playerStateMachine.LedgeDetector.helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

            start += direction * distance2;
            direction = -Vector3.up;

            // Check for the ground or a ledge
            if (Physics.Raycast(start, direction, out hit, distance2, playerStateMachine.LedgeDetector.climbableLayer))
            {
                float angle = Vector3.Angle(-playerStateMachine.LedgeDetector.helper.forward, hit.normal);
                if (angle < 40)
                {
                    playerStateMachine.LedgeDetector.helper.position = OffsetPosition(start, hit.point);
                    playerStateMachine.LedgeDetector.helper.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }
            }

            return false;
        }

        private bool IsGroundedOrOutOfWall()
        {
            // Check if the player is grounded
            Vector3 origin = playerStateMachine.transform.position - playerStateMachine.transform.forward + Vector3.up;
            bool isGrounded = Physics.Raycast(origin, Vector3.down, 0.5f);

            Vector3 start = playerStateMachine.transform.position - playerStateMachine.transform.forward;
            start.y += 1.5f;
            Vector3 direction = playerStateMachine.transform.forward;

            // Check if the player is touching a wall
            RaycastHit hit;
            bool isTouchingWall = Physics.SphereCast(start, 1f, direction, out hit, 3f, playerStateMachine.LedgeDetector.climbableLayer);
            if (!isTouchingWall)
            {
                Debug.Log("Mot touching wall");
            }
            if (isGrounded)
            {
                Debug.Log("grounded");
            }
            return isGrounded || !isTouchingWall;
        }
    }
}