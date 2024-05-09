using System.Linq;
using TPC.Combat;
using TPC.Movement;
using UnityEngine;

namespace TPC.StateMachine
{
    public class PlayerStateMachine : StateMachine
    {
        [field: SerializeField] public InputReader InputReader { get; private set; }
        [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
        [field: SerializeField] public float TargetingMovementSpeed { get; private set; }
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField] public Grabber Grabber { get; private set; }
        [field: SerializeField] public LedgeDetector LedgeDetector { get; private set; }
        [field: SerializeField] public float BlockingWalkSpeed { get; private set; }
        [field: SerializeField] public float DodgeDuration { get; private set; }
        [field: SerializeField] public float DodgeLength { get; private set; }
        [field: SerializeField] public Stamina Stamina { get; private set; }
        [field: SerializeField] public float BlockStaminaCost { get; private set; }
        [field: SerializeField] public float DodgeStaminaCost { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; } = 10f;
        [field: SerializeField] public float JumpHorizontalForce { get; private set; } = 10f;
        [field: SerializeField] public Vector3 PullUpOffset { get; private set; } = new Vector3(0f, 2.325f, 0.65f);
        [SerializeField] private GameObject[] tools;
        public Transform MainCameraTransform { get; private set; }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (InputReader == null)
            {
                InputReader = GetComponent<InputReader>();
            }
            if (Targeter == null)
            {
                Targeter = GetComponentsInChildren<Targeter>().FirstOrDefault();
            }
            if (Grabber == null)
            {
                Grabber = GetComponent<Grabber>();
            }
            if (Stamina == null)
            {
                Stamina = GetComponent<Stamina>();
            }
        }

#endif

        private void Start()
        {
            MainCameraTransform = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Locked;
            SwitchState(new PlayerFreeLookState(this));
        }

        private void OnEnable()
        {
            Health.OnTakeDamage += HandleTakeDamage;
            Health.OnDie += HandleDie;
            InputReader.GrabEvent += HandleGrab;
        }

        private void OnDisable()
        {
            Health.OnTakeDamage -= HandleTakeDamage;
            Health.OnDie -= HandleDie;
            InputReader.GrabEvent -= HandleGrab;
        }

        private void HandleTakeDamage(Transform damageSource)
        {
            SwitchState(new PlayerImpactState(this, damageSource));
        }

        private void HandleDie()
        {
            SwitchState(new PlayerDeadState(this));
        }

        private void HandleGrab()
        {
        }
        public void EnableTools(bool enable)
        {
            foreach (var tool in tools)
            {
                tool.SetActive(enable);
            }
        }
    }
}