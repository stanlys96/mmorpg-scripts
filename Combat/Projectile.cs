using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat {
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject impactEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] gameObjectHit = null;
        [SerializeField] float lifeAfterImpact = 0.2f;

        Health target = null;
        float damage = 0f;

        void Start() {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if (isHoming && !target.IsDead()) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        public void SetTarget(Health target, float damage) {
            this.target = target;
            this.damage = damage;
            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider capsule = target.GetComponent<CapsuleCollider>();
            if (capsule == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * capsule.height / 2;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            speed = 0;
            if (impactEffect != null) {
                GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            }
            target.TakeDamage(damage);
            foreach (GameObject hitObject in gameObjectHit)
            {
                Destroy(hitObject);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}