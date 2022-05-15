using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes {
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;

        // Update is called once per frame
        void Update()
        {
            if (Mathf.Approximately(healthComponent.GetFraction(), 0)) {
                canvas.enabled = false;
            } else {
                canvas.enabled = true;
            }
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}