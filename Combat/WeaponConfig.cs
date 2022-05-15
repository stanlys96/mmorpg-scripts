using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
   [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject {
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float attackRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "WEAPON";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator) {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab != null) {
                weapon = Instantiate(equippedPrefab, GetHandTransform(rightHand, leftHand));
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            } else if (overrideController != null) {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage) {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, calculatedDamage, instigator);
        }

        public float GetAttackRange() {
            return attackRange;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetWeaponDamage() {
            return weaponDamage;
        }
    } 
}
