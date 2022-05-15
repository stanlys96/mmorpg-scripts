using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Movement;
using RPG.Attributes;

namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] float hideTime = 5f;

        const string category = "pickup";

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject) {
            if (weapon != null) {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0) {
                subject.GetComponent<Health>().RestoreHealth(healthToRestore);
            }
            StartCoroutine(HideForSeconds(hideTime));
        }

        public bool HandleRaycast(PlayerController callingController) {
            if (Input.GetMouseButton(1)) {
                callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
            }
            return true;
        }

        public CursorType GetCursorType() {
            return CursorType.Pickup;
        }

        private IEnumerator HideForSeconds(float time) {
            ShowPickup(false);
            yield return new WaitForSeconds(time);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow) {
            GetComponent<Collider>().enabled = shouldShow;
            // for (int i = 0; i < transform.childCount; i++)
            // {
            //     transform.GetChild(i).gameObject.SetActive(shouldShow);
            // }
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}