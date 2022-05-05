using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Stats {
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        // Start is called before the first frame update
        void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}