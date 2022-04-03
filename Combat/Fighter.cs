using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float attackRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float speedFraction = 0.9f;
        float timeSinceLastAttack = Mathf.Infinity;

        Health target;

        private void Update() 
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;
            if (!GetIsInRange()) 
            {
                GetComponent<Mover>().MoveTo(target.transform.position, speedFraction);
            } 
            else 
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks) {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack() {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange() {
            return Vector3.Distance(transform.position, target.transform.position) < attackRange;
        }
        public void Attack(GameObject combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget) {
            if (combatTarget == null) return false;
            Health health = combatTarget.GetComponent<Health>();
            return health != null && health.IsDead() == false;
        }

        // Animation Event
        void Hit() {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }

        public void Cancel() {
            StopAttack();
            target = null;
        }

        private void StopAttack() {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}