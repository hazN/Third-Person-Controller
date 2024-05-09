using UnityEngine;

namespace TPC.Combat
{
    public class Grabber : MonoBehaviour
    {
        public bool IsGrabbing { get; private set; }
        private Transform target;
        private Animator animator;
        private float draggingSpeed = 1f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Grab(Transform target, Transform hand)
        {
            this.target = target;
            IsGrabbing = true;
            target.GetComponent<Ragdoll>().ToggleRagdoll(true);
            target.SetParent(hand);
        }
        private void Update()
        {
            if (IsGrabbing && Vector3.Distance(transform.position, target.position) > 1.5f)
            {
                // Calculate the direction from the target to the grabber
                Vector3 directionToGrabber = transform.position - target.position;

                // Normalize the direction vector (to get a unit vector pointing towards the grabber)
                directionToGrabber.Normalize();

                // Calculate the target's new position towards the grabber
                Vector3 newTargetPosition = target.position + directionToGrabber * (Time.deltaTime * draggingSpeed);

                Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
                if (targetRigidbody != null)
                {
                    targetRigidbody.MovePosition(newTargetPosition);
                }
                else
                {
                    // If the target doesn't have a Rigidbody, move it directly (less realistic)
                    target.position = newTargetPosition;
                }
            }
        }
        public void Release()
        {
            IsGrabbing = false;
            target.GetComponent<Ragdoll>().ToggleRagdoll(false);
            target.SetParent(null);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (IsGrabbing)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, target.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, target.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }
}