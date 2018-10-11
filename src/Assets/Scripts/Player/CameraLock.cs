using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

namespace Cyan
{
    /// <summary>
    /// When interacting with drawers player camera will lock in position until the drawer is released
    /// </summary>
    public class CameraLock : MonoBehaviour
    {
        private FirstPersonController fpsController;
        private Interact player;
        // Use this for initialization
        void Awake()
        {
            fpsController = GetComponent<FirstPersonController>();
            player = GetComponentInChildren<Interact>();
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (player.currentAction == InteractStates.isPulling && player.greaterControl)
            {
                fpsController.enabled = false;
            }

            else
            {
                fpsController.enabled = true;
            }

        }
    }
}
