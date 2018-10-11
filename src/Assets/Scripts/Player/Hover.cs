using UnityEngine;
using System.Collections;

namespace Cyan
{
    /// <summary>
    /// Attached to player, shoots raycast every frame to check for interactability 
    /// </summary>
    public class Hover : MonoBehaviour
    {

        private const float rayDistance = 2f;
        private const float holdDistance = 1f;
        private Interact player;

        public bool canInteract = false;

        /// <summary>
        /// Called every frame- shoots out a raycast and checks if anything within 2 units is interactable or not- will change the canInteract bool
        /// </summary>
        /// 
        void Start()
        {
            player = FindObjectOfType<Interact>();
        }
        public void InspectFront()
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Vector3 target = Camera.main.transform.forward * rayDistance;
            RaycastHit targetHit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out targetHit, rayDistance))
            {
                if (player.currentAction != InteractStates.isNotInteracting && player.selectedItem != targetHit.collider.gameObject)
                {
                    canInteract = false;
                }
                else if (targetHit.collider.gameObject.tag.Contains("object"))
                {
                    canInteract = true;
                }

                else
                {
                    canInteract = false;
                }

            }
        }


        // Update is called once per frame
        void Update()
        {
            canInteract = false;
            InspectFront();
        }

    }
}

