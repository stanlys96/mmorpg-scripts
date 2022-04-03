using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine.AI;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        GameObject player;
        Fighter fighter;
        Health health;
        Vector3 guardPosition;
        Mover mover;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        [SerializeField] float suspiscionTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float dwellingTime = 3f;
        [Range(0, 1)]
        [SerializeField] float speedFraction = 0.2f;
        private int waypointIndex = 0;
        private float patrolSpeed = 3f;
        private float chaseSpeed = 5f;

        void Start() {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player)) {
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspiscionTime) {
                SuspicionBehaviour();
            } else {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void AttackBehaviour() {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            GetComponent<NavMeshAgent>().speed = chaseSpeed;
            GetComponent<Animator>().SetFloat("forwardSpeed", chaseSpeed);
        }

        private void SuspicionBehaviour() {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour() {
            GetComponent<NavMeshAgent>().speed = patrolSpeed;
            GetComponent<Animator>().SetFloat("forwardSpeed", patrolSpeed);
            Vector3 nextPosition = guardPosition;

            if (patrolPath != null) {
                if (AtWaypoint()) {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > dwellingTime) {
                mover.StartMoveAction(nextPosition, speedFraction);
            }
        }

        private bool AtWaypoint() {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint() {
            waypointIndex = patrolPath.GetNextIndex(waypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(waypointIndex);
        }

        private bool InAttackRangeOfPlayer() {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}