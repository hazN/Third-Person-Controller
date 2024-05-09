using System;
using TPC.StateMachine;
using UnityEngine;

namespace TPC
{
    public class Health : MonoBehaviour
    {
        public event Action<Transform> OnTakeDamage;
        public event Action OnDie;

        public bool IsDead => currentHealth <= 0;

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float parryWindow = 0.2f;
        [SerializeField] private float healthRegenRate = 1.5f;
        [SerializeField] private float timeBeforeRegen = 5f;

        private float currentHealth;
        private float timeSinceLastDamage;
        private float timeSinceStartedBlocking;
        private bool isBlocking;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if (ShouldRegenerateHealth())
            {
                RegenerateHealth();
            }
        }

        private bool ShouldRegenerateHealth()
        {
            return Time.time - timeSinceLastDamage > timeBeforeRegen && currentHealth < maxHealth;
        }

        private void RegenerateHealth()
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healthRegenRate * Time.deltaTime);
        }

        public bool DealDamage(float damage, Transform damageSource)
        {
            if (isBlocking && IsWithinParryWindow())
            {
                PerformParry(damageSource);
                return false;
            }

            TakeDamage(damage, damageSource);
            return IsDead;
        }

        private bool IsWithinParryWindow()
        {
            return Time.time - timeSinceStartedBlocking < parryWindow;
        }

        private void PerformParry(Transform damageSource)
        {
            Debug.Log("Parried!");
            EnemyStateMachine enemyStateMachine = damageSource.GetComponent<EnemyStateMachine>();
            enemyStateMachine.SwitchState(new EnemyImpactState(enemyStateMachine, transform));
        }

        private void TakeDamage(float damage, Transform damageSource)
        {
            Debug.Log("Taking damage: " + damage);
            timeSinceLastDamage = Time.time;
            currentHealth = Mathf.Max(0, currentHealth - damage);

            OnTakeDamage?.Invoke(damageSource);

            if (IsDead)
            {
                OnDie?.Invoke();
            }
        }

        public void SetBlocking(bool isBlocking)
        {
            this.isBlocking = isBlocking;
            timeSinceStartedBlocking = Time.time;
        }

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }
    }
}
