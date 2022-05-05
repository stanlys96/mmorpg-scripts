using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70f;

        LazyValue<float> healthPoints;

        private bool isDead = false;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start() {
            healthPoints.ForceInit();
        }

        private void OnEnable() {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead() {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage) {
            print(gameObject.name + " took damage: " + damage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (healthPoints.value <= 0) {
                RewardExperience(instigator);
                Die();
            }
        }

        public float GetPercentage() {
            return 100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die() {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("death");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }

        public void RewardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPoints() {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void RegenerateHealth() {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public object CaptureState() {
            return healthPoints.value;
        }

        public void RestoreState(object state) {
            float health = (float)state;
            healthPoints.value = health;
            if (healthPoints.value <= 0) {
                Die();
            }
        }
    }
}