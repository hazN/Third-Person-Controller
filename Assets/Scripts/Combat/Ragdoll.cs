using UnityEngine;

namespace TPC.Combat
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        private Collider[] colliders;
        private Rigidbody[] rigidbodies;

        private void Start()
        {
            colliders = GetComponentsInChildren<Collider>(true);
            rigidbodies = GetComponentsInChildren<Rigidbody>(true);

            ToggleRagdoll(false);
        }

        public void ToggleRagdoll(bool isRagdoll)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.CompareTag("Ragdoll"))
                {
                    collider.enabled = isRagdoll;
                }
            }

            foreach (var rb in rigidbodies)
            {
                if (rb.gameObject.CompareTag("Ragdoll"))
                {
                    rb.isKinematic = !isRagdoll;
                    rb.useGravity = isRagdoll;
                }
            }

            animator.enabled = !isRagdoll;
            characterController.enabled = !isRagdoll;
        }

        public void ToggleColliders(bool isCollidersEnabled)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = isCollidersEnabled;
            }
        }
    }
}