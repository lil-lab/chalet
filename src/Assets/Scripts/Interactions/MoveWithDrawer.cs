using UnityEngine;
using System.Collections;

namespace Cyan
{
    /// <summary>
    /// Attached to each drawer. Items placed on drawers will move with the drawer
    /// </summary>
    public class MoveWithDrawer : MonoBehaviour
    {
        private BoxCollider bC;
        private Vector3 originalObjectBC;

        // Use this for initialization
        void Start()
        {
            bC = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Detect when an object is placed in the drawer and make the item move with the drawer
        /// </summary>
        /// <param name="obj">obj that triggers box collider</param>
        void OnTriggerEnter(Collider obj)
        {
            try
            {
                if (obj.tag.Contains("object") && obj.transform.parent == null)
                {
                    obj.transform.parent = bC.transform;
                }
            }

            catch
            {

            }

        }

        /// <summary>
        /// If an item leaves the bottom of a drawer (i.e bounces out) the object will be unparented from the drawer so that it doesnt not continue to move with the drawer
        /// </summary>
        /// <param name="obj"></param>
        void OnTriggerExit(Collider obj)
        {
            try
            {
                if (obj.transform.parent == bC.transform)
                {
                    obj.transform.parent = null;
                }
            }

            catch
            {
                // item left already parented to the user
            }
        }



    }
}
