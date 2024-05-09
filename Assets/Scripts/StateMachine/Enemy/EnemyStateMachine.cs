using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TPC.Combat;
using TPC.Movement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace TPC.StateMachine
{
    public class EnemyStateMachine : StateMachine
    {
        [BoxGroup("Components")][field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public Target Target { get; private set; }
        [BoxGroup("Parameters")][field: SerializeField] public float DetectionRange { get; private set; }
        [BoxGroup("Parameters")][field: SerializeField] public float MovementSpeed { get; private set; }
        [BoxGroup("Parameters")][field: SerializeField] public float AttackRange { get; private set; }
        public Health Player { get; private set; }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Agent == null)
            {
                Agent = GetComponent<NavMeshAgent>();
            }
            if (Target == null)
            {
                Target = GetComponent<Target>();
            }
        }
#endif
        private void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

            Agent.updatePosition = false;
            Agent.updateRotation = false;
        }
        private void Start()
        {
            SwitchState(new EnemyIdleState(this));
        }
        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDie += HandleDie;
        }
        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDie -= HandleDie;
        }
        private void HandleTakeDamage(Transform damageSource)
        {
            SwitchState(new EnemyImpactState(this, damageSource));
        }
        private void HandleDie()
        {
            SwitchState(new EnemyDeadState(this));
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DetectionRange);
        }
    }
}