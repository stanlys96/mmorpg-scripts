using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Core {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints = 100f;
        private bool isDead = false;

        public bool IsDead() {
            return isDead;
        }

        public void TakeDamage(float damage) {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0) {
                Die();
            }
        }

        private void Die() {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("death");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }

        public object CaptureState() {
            return healthPoints;
        }

        public void RestoreState(object state) {
            float health = (float)state;
            healthPoints = health;
            if (healthPoints <= 0) {
                Die();
            }
        }
    }
}