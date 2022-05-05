using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats {
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0f;

        public event Action onExperienceGained;

        public void GainExperience(float experience) {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetPoints() {
            return experiencePoints;
        }

        public object CaptureState() {
            return experiencePoints;
        }

        public void RestoreState(object state) {
            float experience = (float) state;
            experiencePoints = experience;
        }
    }
}