using UnityEngine;

namespace Cyan
{
    /// <summary>
    /// Easy access for bot actions?
    /// </summary>
    public class Actions : MonoBehaviour
    {
        [Tooltip("Found on Awake- no need to drag in inspector")]
        public Interact interact;

        // Use this for initialization
        void Start()
        {
            try
            {
                interact.GetComponent<Interact>();
            }
            catch { }
        }

        /// <summary>
        /// Calls the ShootRay() function from the Interact script, shoots an interaction check 
        /// So if you are holding an item, you should release it, and if you arent, checks if there is anything to grab
        /// If you can grab an item, the item will be parented to the player/agent
        /// </summary>
        public void Interact()
        {
            if (interact != null)
            {
                interact.ShootRay();
            }
        }

        /// <summary>
        ///  Forcefully looks for an item in the scene by its name and makes it the selected/parented item 
        /// </summary>
        /// <param name="itemName">name of the item in the scene</param>
        public void ForceGrab(string itemName)
        {
            interact.Freeze(itemName);
        }

        /// <summary>
        ///  Forecfully drops the item being held, even if the drop location is not a good spot
        /// </summary>
        public void ForceDrop()
        {
            interact.Drop();
        }
    }
}
