using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement {
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier {
            A, B, C, D, E
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destinationIdentifier;
        [SerializeField] float fadeOutTime = 1.5f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeWaitTime = 1.5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {
            if (sceneToLoad < 0) {
                Debug.LogError("Scene to load not set.");
                yield break;
            }
            Fader fader = FindObjectOfType<Fader>();
            DontDestroyOnLoad(gameObject);
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);
        }

        private Portal GetOtherPortal() {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destinationIdentifier != this.destinationIdentifier) continue;
                return portal;
            }
            return null;
        }

        private void UpdatePlayer(Portal otherPortal) {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }
    }
}