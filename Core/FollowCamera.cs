using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
  public class FollowCamera : MonoBehaviour
  {
    [SerializeField] Transform target;
    [SerializeField] private Camera cam;

    private Vector3 previousPosition;

    private float cameraOffset = 1f;

    private void Start()
    {
      cameraOffset = cam.transform.position.z - transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
      transform.position = target.position;
      if (Input.GetMouseButtonDown(0))
      {
        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
      }

      if (Input.GetMouseButton(0))
      {
        Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);
        cam.transform.position = target.position;
        cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
        cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
        cam.transform.Translate(0, 0, -9);

        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
      }

    }
  }
}