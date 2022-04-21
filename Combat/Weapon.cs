using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
   [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float attackRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "WEAPON";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator) {
            DestroyOldWeapon(rightHand, leftHand);
            if (equippedPrefab != null) {
                GameObject weapon = Instantiate(equippedPrefab, GetHandTransform(rightHand, leftHand));
                weapon.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            } else if (overrideController != null) {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand) {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null) {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYED";
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public Transform GetHandTransform(Transform rightHand, Transform leftHand) {
            Transform handTransform;
            handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target) {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }

        public float GetAttackRange() {
            return attackRange;
        }

        public float GetWeaponDamage() {
            return weaponDamage;
        }
    } 
}
