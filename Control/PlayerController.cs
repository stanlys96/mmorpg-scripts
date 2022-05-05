using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using RPG.Core;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour
    {
        Health health;

        void Awake() {
            health = GetComponent<Health>();
        }
        
        void Update() {
            if (health.IsDead()) return;
            if (InteractWithCombat()) {
                return;
            }
            if (InteractWithMovement()) {
                return;
            }
        }

        private bool InteractWithCombat() {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
                if (GetComponent<Fighter>().CanAttack(target.gameObject) == false) continue;
                if (Input.GetMouseButton(1)) {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement() {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit) {
                if (Input.GetMouseButton(1))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point, 1f);
                }
            }
            return hasHit;
        }

        private Ray GetMouseRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}