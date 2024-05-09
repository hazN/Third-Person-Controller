using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;

namespace TPC.Movement
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class LedgeDetector : MonoBehaviour
    {
        public bool IsClimbing { get; private set; }

        public event Action OnClimb;

        [field: SerializeField] public float positionOffset { get; private set; } = 0.5f;
        [field: SerializeField] public float rayToWall { get; private set; } = 1f;
        [field: SerializeField] public float rayToMoveDir { get; private set; } = 0.5f;
        [field: SerializeField] public float offsetFromWall { get; private set; } = 0.3f;
        [field: SerializeField] public float climbSpeed { get; private set; } = 3f;
        [field: SerializeField] public float rotateSpeed { get; private set; } = 5f;
        [field: SerializeField] public float staminaCost { get; private set; } = 15f;
        [field: SerializeField] public LayerMask climbableLayer { get; private set; }
        public IKSnapshot baseIKSnapshot;
        public Transform helper;
        public ClimbingAnimator climbingAnimator;

        private void Start()
        {
            helper = new GameObject().transform;
            helper.name = "Climb Helper";
            climbingAnimator.Init(this);
        }
        public bool TryClimb(out RaycastHit res)
        {
            Vector3 origin = transform.position;
            origin.y += 1.5f;

            Vector3 direction = transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, 1f))
            {
                res = hit;
                return true;
            }
            res = default;
            return false;
        }
        public void SetClimbing(bool isClimbing)
        {
            IsClimbing = isClimbing;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 11)
            {
                OnClimb?.Invoke();
            }
        }
    }
    [System.Serializable]
    public class IKSnapshot
    {
        public Vector3 rh, lh, rf, lf;
        public void Store(Transform rh, Transform lh, Transform rf, Transform lf)
        {
            this.rh = rh.position;
            this.lh = lh.position;
            this.rf = rf.position;
            this.lf = lf.position;
        }
        public void Apply(Transform rh, Transform lh, Transform rf, Transform lf)
        {
            rh.position = this.rh;
            lh.position = this.lh;
            rf.position = this.rf;
            lf.position = this.lf;
        }
    }
}