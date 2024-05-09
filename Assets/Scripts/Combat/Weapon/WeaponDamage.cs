using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TPC.Movement;
using UnityEngine;
namespace TPC.Combat
{
    public class WeaponDamage : MonoBehaviour
    {
        [SerializeField] private Collider myCollider;
        private List<Collider> collidedWith = new List<Collider>();
        private float damage = 10;
        private float knockback = 2;
        private void OnEnable()
        {
            collidedWith.Clear();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other == myCollider || collidedWith.Contains(other)) return;

            collidedWith.Add(other);

            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                health.DealDamage(damage, myCollider.transform);
            }
            ForceReceiver forceReceiver = other.GetComponent<ForceReceiver>();
            if (forceReceiver != null)
            {
                forceReceiver.AddForce((other.transform.position - myCollider.transform.position).normalized * knockback);
            }
        }
        public void SetAttack(float damage, float knockback)
        {
            this.damage = damage;
            this.knockback = knockback;
        }
    }
}