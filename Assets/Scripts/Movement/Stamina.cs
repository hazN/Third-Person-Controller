using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TPC.Movement
{
    public class Stamina : MonoBehaviour
    {
        [field: SerializeField] public float MaxStamina { get; private set; }
        [field: SerializeField] public float CurrentStamina { get; private set; }
        [field: SerializeField] public float StaminaRegenRate { get; private set; }
        private float timeWhileRegenerated = 0;
        void Start()
        {
            CurrentStamina = MaxStamina;
        }

        void Update()
        {
            timeWhileRegenerated += Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina + StaminaRegenRate * Time.deltaTime * timeWhileRegenerated, 0, MaxStamina);
        }
        public bool TryUseStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                CurrentStamina -= amount;
                timeWhileRegenerated = 0;
                return true;
            }
            return false;
        }
        public bool CanUseStamina(float amount)
        {
            return CurrentStamina >= amount;
        }
    }
}