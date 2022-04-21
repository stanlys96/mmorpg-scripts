using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] float hideTime = 5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(hideTime));
            }
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