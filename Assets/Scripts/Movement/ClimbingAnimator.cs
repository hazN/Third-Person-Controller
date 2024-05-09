using System.Collections.Generic;
using UnityEngine;

namespace TPC.Movement
{
    public class ClimbingAnimator : MonoBehaviour
    {
        private readonly int climbIdleHash = Animator.StringToHash("Climbing");
        private readonly int climbUpHash = Animator.StringToHash("ClimbingUp");
        private readonly int climbDownHash = Animator.StringToHash("ClimbingDown");
        private readonly int mirrorHash = Animator.StringToHash("mirror");

        private Animator animator;
        private LedgeDetector detector;
        private InputReader inputReader;
        private Transform helper;

        [SerializeField] private float lerpSpeed = 5f;
        private const float weight = 1f;

        private IKSnapshot baseIK = new IKSnapshot();
        private IKSnapshot currentIK = new IKSnapshot();
        private IKSnapshot nextIK = new IKSnapshot();
        private IKGoals goals = new IKGoals();
        private float w_rh, w_lh, w_rf, w_lf;
        private Vector3 rh, lh, rf, lf;


        private bool isMirror, isLeft;
        private bool firstTime = false;

        private List<IKState> states = new List<IKState>();

        public void Init(LedgeDetector ledgeDetector)
        {
            detector = ledgeDetector;
            animator = ledgeDetector.GetComponent<Animator>();
            inputReader = ledgeDetector.GetComponent<InputReader>();
            baseIK = ledgeDetector.baseIKSnapshot;
            helper = ledgeDetector.helper;
        }

        public void CreatePositions(Vector3 origin, Vector3 direction, bool isMidClimb)
        {
            HandleAnimation(isMidClimb);
            UpdateGoals();

            IKSnapshot snapshot = CreateSnapshot(origin);
            CopySnapshot(ref currentIK, snapshot);

            SetIKState(isMidClimb, goals.lf, currentIK.lf, AvatarIKGoal.LeftFoot);
            SetIKState(isMidClimb, goals.rf, currentIK.rf, AvatarIKGoal.RightFoot);
            SetIKState(isMidClimb, goals.lh, currentIK.lh, AvatarIKGoal.LeftHand);
            SetIKState(isMidClimb, goals.rh, currentIK.rh, AvatarIKGoal.RightHand);

            UpdateIKWeight(AvatarIKGoal.LeftFoot, weight);
            UpdateIKWeight(AvatarIKGoal.RightFoot, weight);
            UpdateIKWeight(AvatarIKGoal.LeftHand, weight);
            UpdateIKWeight(AvatarIKGoal.RightHand, weight);
        }

        public void SetStartingPosition(Vector3 origin, Vector3 direction, bool isMidClimb)
        {
            CopySnapshot(ref nextIK, baseIK);
            baseIK.lf.y = 0;
            baseIK.rf.y = 0;
            CreatePositions(origin, direction, true);
            CreatePositions(origin, direction, isMidClimb);
            firstTime = true;
        }

        private void UpdateGoals()
        {
            isLeft = inputReader.MovementValue.x < 0;
            if (inputReader.MovementValue.x != 0)
            {
                goals.lh = isLeft;
                goals.rh = !isLeft;
                goals.lf = isLeft;
                goals.rf = !isLeft;
            }
            else
            {
                bool isEnabled = isMirror;
                if (inputReader.MovementValue.y < 0)
                {
                    isEnabled = !isMirror;
                }
                goals.lh = isEnabled;
                goals.rh = !isEnabled;
                goals.lf = isEnabled;
                goals.rf = !isEnabled;
            }
        }

        private void HandleAnimation(bool isMidClimb)
        {
            Vector3 direction = inputReader.MovementValue;
            if (isMidClimb)
            {
                if (direction.y != 0)
                {
                    if (direction.x == 0)
                    {
                        isMirror = !isMirror;
                        animator.SetBool(mirrorHash, isMirror);
                    }
                    else
                    {
                        if (direction.y < 0)
                        {
                            isMirror = (direction.x > 0);
                            animator.SetBool(mirrorHash, isMirror);
                        }
                        else
                        {
                            isMirror = (direction.x < 0);
                            animator.SetBool(mirrorHash, isMirror);
                        }
                    }
                }
                else
                {
                    isMirror = direction.x < 0;
                }

                animator.CrossFade(climbUpHash, 0.2f);
            }
            else
            {
                animator.CrossFade(climbIdleHash, 0.2f);
            }
        }

        public IKSnapshot CreateSnapshot(Vector3 info)
        {
            IKSnapshot snapshot = new IKSnapshot();
            Vector3 _lh = LocalToWorld(baseIK.lh);
            snapshot.lh = GetActualPosition(_lh, AvatarIKGoal.LeftHand);

            Vector3 _rh = LocalToWorld(baseIK.rh);
            snapshot.rh = GetActualPosition(_rh, AvatarIKGoal.RightHand);

            Vector3 _lf = LocalToWorld(baseIK.lf);
            snapshot.lf = GetActualPosition(_lf, AvatarIKGoal.LeftFoot);

            Vector3 _rf = LocalToWorld(baseIK.rf);
            snapshot.rf = GetActualPosition(_rf, AvatarIKGoal.RightFoot);
            return snapshot;
        }

        public void CopySnapshot(ref IKSnapshot to, IKSnapshot from)
        {
            to.rh = from.rh;
            to.lh = from.lh;
            to.rf = from.rf;
            to.lf = from.lf;
        }

        public void SetIKState(bool isMidClimb, bool isTrue, Vector3 position, AvatarIKGoal goal)
        {
            if (isMidClimb)
            {
                if (isTrue)
                {
                    Vector3 pos = GetActualPosition(position, goal);
                    UpdateIKPosition(goal, pos);
                }
            }
            else
            {
                if (!isTrue)
                {
                    Vector3 pos = GetActualPosition(position, goal);
                    UpdateIKPosition(goal, pos);
                }
            }
        }

        public void UpdateIKPosition(AvatarIKGoal goal, Vector3 position)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    lf = position;
                    break;

                case AvatarIKGoal.RightFoot:
                    rf = position;
                    break;

                case AvatarIKGoal.LeftHand:
                    lh = position;
                    break;

                case AvatarIKGoal.RightHand:
                    rh = position;
                    break;

                default:
                    break;
            }
        }

        public void UpdateIKWeight(AvatarIKGoal goal, float weight)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    w_lf = weight;
                    break;

                case AvatarIKGoal.RightFoot:
                    w_rf = weight;
                    break;

                case AvatarIKGoal.LeftHand:
                    w_lh = weight;
                    break;

                case AvatarIKGoal.RightHand:
                    w_rh = weight;
                    break;

                default:
                    break;
            }
        }

        private Vector3 LocalToWorld(Vector3 targetPos)
        {
            Vector3 r = helper.position;
            r += helper.right * targetPos.x;
            r += helper.up * targetPos.y;
            r += helper.forward * targetPos.z;
            return r;
        }

        private Vector3 GetActualPosition(Vector3 position, AvatarIKGoal goal)
        {
            Vector3 result = position;
            Vector3 origin = position;
            Vector3 direction = helper.forward;
            origin += -(direction * 0.1f);

            bool isHit = false;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, 0.5f))
            {
                result = hit.point + (hit.normal * 0.1f);
                isHit = true;

                if (goal == AvatarIKGoal.LeftFoot || goal == AvatarIKGoal.RightFoot)
                {
                    if (hit.point.y > transform.position.y - 0.1f)
                    {
                        isHit = false;
                    }
                }
            }

            if (!isHit)
            {
                switch (goal)
                {
                    case AvatarIKGoal.LeftFoot:
                        result = LocalToWorld(baseIK.lf);
                        break;

                    case AvatarIKGoal.RightFoot:
                        result = LocalToWorld(baseIK.rf);
                        break;

                    case AvatarIKGoal.LeftHand:
                        result = LocalToWorld(baseIK.lh);
                        break;

                    case AvatarIKGoal.RightHand:
                        result = LocalToWorld(baseIK.rh);
                        break;

                    default:
                        break;
                }
            }
            return result;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (detector.IsClimbing)
            {
                if (firstTime)
                {
                    firstTime = false;
                    SetIKInfo(AvatarIKGoal.LeftFoot, lf, w_lf, false);
                    SetIKInfo(AvatarIKGoal.RightFoot, rf, w_rf, false);
                    SetIKInfo(AvatarIKGoal.LeftHand, lh, w_lh, false);
                    SetIKInfo(AvatarIKGoal.RightHand, rh, w_rh, false);
                    CopySnapshot(ref baseIK, nextIK);
                }
                else
                {
                    SetIKInfo(AvatarIKGoal.LeftFoot, lf, w_lf, true);
                    SetIKInfo(AvatarIKGoal.RightFoot, rf, w_rf, true);
                    SetIKInfo(AvatarIKGoal.LeftHand, lh, w_lh, true);
                    SetIKInfo(AvatarIKGoal.RightHand, rh, w_rh, true);
                }
            }
        }

        private void SetIKInfo(AvatarIKGoal goal, Vector3 position, float weight, bool lerp)
        {
            IKState ikState = GetIKStates(goal);
            if (ikState == null)
            {
                ikState = new IKState();
                ikState.goal = goal;
                states.Add(ikState);
            }

            if (weight == 0)
            {
                ikState.isSet = false;
            }

            if (!ikState.isSet)
            {
                ikState.position = GoalToBodyBones(goal).position;
                ikState.isSet = true;
            }

            ikState.weight = weight;
            if (lerp)
            {
                ikState.position = Vector3.Lerp(ikState.position, position, Time.deltaTime * lerpSpeed);
            }
            else
            {
                ikState.position = position;
            }

            animator.SetIKPosition(goal, ikState.position);
            animator.SetIKPositionWeight(goal, ikState.weight);
        }

        private Transform GoalToBodyBones(AvatarIKGoal goal)
        {
            switch (goal)
            {
                case AvatarIKGoal.LeftFoot:
                    return animator.GetBoneTransform(HumanBodyBones.LeftFoot);

                case AvatarIKGoal.RightFoot:
                    return animator.GetBoneTransform(HumanBodyBones.RightFoot);

                case AvatarIKGoal.LeftHand:
                    return animator.GetBoneTransform(HumanBodyBones.LeftHand);

                default:
                case AvatarIKGoal.RightHand:
                    return animator.GetBoneTransform(HumanBodyBones.RightHand);
            }
        }

        private IKState GetIKStates(AvatarIKGoal goal)
        {
            IKState state = null;
            foreach (IKState iKState in states)
            {
                if (iKState.goal == goal)
                {
                    state = iKState;
                    break;
                }
            }
            return state;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lf, 0.1f);
            Gizmos.DrawSphere(rf, 0.1f);
            Gizmos.DrawSphere(lh, 0.1f);
            Gizmos.DrawSphere(rh, 0.1f);
        }

        private class IKState
        {
            public AvatarIKGoal goal;
            public Vector3 position;
            public float weight;
            public bool isSet;
        }
    }

    public class IKGoals
    {
        public bool rh, lh, rf, lf;
    }
}