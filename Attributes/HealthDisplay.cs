using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes {
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        // Start is called before the first frame update
        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}