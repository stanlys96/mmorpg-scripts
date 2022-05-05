using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            Health health = fighter.GetTarget();
            if (health != null) {
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
            } else {
                GetComponent<Text>().text = "N/A";
            }
        }
    }
}