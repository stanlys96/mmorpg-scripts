using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine.AI;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        GameObject player;
        Fighter fighter;
        Health health;
        LazyValue<Vector3> guardPosition;
        Mover mover;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;

        [SerializeField] float suspiscionTime = 5f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float dwellingTime = 3f;
        [Range(0, 1)]
        [SerializeField] float speedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5f;
        private int waypointIndex = 0;
        private float patrolSpeed = 3f;
        private float chaseSpeed = 5f;

        private void Awake() {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition() {
            return transform.position;
        }

        void Start() {
            guardPosition.ForceInit();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead()) return;
            if (IsAggrevated() && fighter.CanAttack(player)) {
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspiscionTime) {
                SuspicionBehaviour();
            } else {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        public void Aggrevate() {
            timeSinceAggrevated = 0;
        }

        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void AttackBehaviour() {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            GetComponent<NavMeshAgent>().speed = chaseSpeed;
            GetComponent<Animator>().SetFloat("forwardSpeed", chaseSpeed);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies() {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<AIController>() != null) {
                    hit.transform.GetComponent<AIController>().Aggrevate();
                }
            }
        }

        private void SuspicionBehaviour() {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour() {
            GetComponent<NavMeshAgent>().speed = patrolSpeed;
            GetComponent<Animator>().SetFloat("forwardSpeed", patrolSpeed);
            Vector3 nextPosition = guardPosition.value;

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

        private bool IsAggrevated() {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}