using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using RPG.Core;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorMapping[] cursorMappings;
        [SerializeField] float raycastRadius = 1f;

        Health health;

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        void Awake() {
            health = GetComponent<Health>();
        }
        
        void Update() {
            if (InteractWithUI()) {
                SetCursor(CursorType.UI);
                return;
            }
            if (health.IsDead()) {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) {
                return;
            }
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent() {
            RaycastHit[] hits = GetRaycastSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] GetRaycastSorted() {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithMovement() {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit) {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                if (Input.GetMouseButton(1))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
            }
            SetCursor(CursorType.Movement);
            return hasHit;
        }

        private bool RaycastNavMesh(out Vector3 target) {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;
            bool navMeshHasHit = NavMesh.SamplePosition(hit.point, out navMeshHit, 1.0f, NavMesh.AllAreas);
            if (!navMeshHasHit) return false;

            target = navMeshHit.position;

            return true;
        }

        private bool InteractWithUI () {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type) {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private Ray GetMouseRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}