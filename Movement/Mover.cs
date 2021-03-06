using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement {
  public class Mover : MonoBehaviour, IAction
  {
    [SerializeField] float maxSpeed = 5.6f;
    NavMeshAgent navMeshAgent;
    Health health;

    void Start() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<Health>();
    }

    void Update() {
      navMeshAgent.enabled = !health.IsDead();
      UpdateAnimation();
    }

    public void StartMoveAction(Vector3 destination, float speedFraction) {
      GetComponent<ActionScheduler>().StartAction(this);
      MoveTo(destination, speedFraction);
    }

    public void MoveTo(Vector3 destination, float speedFraction) {
      navMeshAgent.destination = destination;
      navMeshAgent.speed = maxSpeed * speedFraction;
      navMeshAgent.isStopped = false;
    }

    public void Cancel() {
      navMeshAgent.isStopped = true;
    }

    void UpdateAnimation() {
      Vector3 velocity = navMeshAgent.velocity;
      Vector3 localVelocity = transform.InverseTransformDirection(velocity);
      float speed = localVelocity.z;
      GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }
  }
}