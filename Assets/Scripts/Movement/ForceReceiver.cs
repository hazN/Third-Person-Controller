using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TPC.Movement
{
    public class ForceReceiver : MonoBehaviour
    {
        [SerializeField] private float drag = 0.3f;
        [SerializeField] private NavMeshAgent agent;
        private CharacterController characterController;
        private float verticalVelocity = 0f;
        private Vector3 impact;
        private Vector3 dampingVelocity;
        public Vector3 Movement => impact + Vector3.up * verticalVelocity;
        public bool enableGravity = true;
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
        }
        private void Update()
        {
            if (enableGravity)
            {
                if (verticalVelocity < 0f && characterController.isGrounded)
                {
                    verticalVelocity = Physics.gravity.y * Time.deltaTime;
                }
                else
                {
                    verticalVelocity += Physics.gravity.y * Time.deltaTime;
                }
            }

            impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

            if (agent != null)
            {
                if (impact.sqrMagnitude < 0.2f * 0.2f)
                {
                    impact = Vector3.zero;
                    agent.enabled = true;
                }
            }
        }
        public void AddForce(Vector3 force)
        {
            impact += force;
            if (agent != null)
            {
                agent.enabled = false;
            }
        }
        public void Jump(float force)
        {
            verticalVelocity += force;
        }
        public void Reset()
        {
            impact = Vector3.zero;
            verticalVelocity = 0f;
        }
    }
}