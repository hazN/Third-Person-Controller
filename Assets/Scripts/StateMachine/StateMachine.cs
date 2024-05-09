using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TPC.Combat;
using TPC.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace TPC.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State currentState;
        [BoxGroup("Components")][field: SerializeField] public Animator Animator { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public CharacterController Controller { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public WeaponHandler WeaponHandler { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public WeaponDamage Weapon { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public Health Health { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public Ragdoll Ragdoll { get; private set; }
        [BoxGroup("Components")][field: SerializeField] public GameObject LeftHand { get; private set; }
        [BoxGroup("Parameters")][field: SerializeField] public Attack[] Attacks { get; private set; }

        [BoxGroup("Parameters")][field: SerializeField] public float RotationDamping { get; private set; }



#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Animator == null)
            {
                Animator = GetComponent<Animator>();
            }
            if (Controller == null)
            {
                Controller = GetComponent<CharacterController>();
            }
            if (ForceReceiver == null)
            {
                ForceReceiver = GetComponent<ForceReceiver>();
            }
            if (WeaponHandler == null)
            {
                WeaponHandler = GetComponent<WeaponHandler>();
            }
            if (Weapon == null)
            {
                Weapon = GetComponentInChildren<WeaponDamage>();
            }
            if (Health == null)
            {
                Health = GetComponent<Health>();
            }
            if (Ragdoll == null)
            {
                Ragdoll = GetComponent<Ragdoll>();
            }
        }
#endif
        private void Update()
        {
            currentState?.Tick(Time.deltaTime);
        }

        public void SwitchState(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }
        private void FootL()
        {

        }
        private void FootR()
        {
        }
        private void Hit()
        {
        }
        private void ActivateHitbox()
        {
            WeaponHandler.EnableWeapon();
        }
        private void DeactivateHitbox()
        {
            WeaponHandler.DisableWeapon();
        }
    }
}