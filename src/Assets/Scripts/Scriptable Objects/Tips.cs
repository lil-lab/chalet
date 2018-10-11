using UnityEngine;

namespace Cyan
{
    [CreateAssetMenu()]
    public class Tips : ScriptableObject
    {

        public bool gotMovementTip = false;
        public bool gotHoldTip = false;
        public bool gotInteractTip = false;
        public bool gotDrawerTip = false;
        public bool gotCabinetTip = false;

        //Tips
        public string movementTip = "Use the WASD keys to move and your mouse to look around and interract";
        public string holdTip = "Press R to rotate an item that you are holding and left click to release the item";
        public string interactTip = "When your pointer turns blue, you can click your left mouse button to interact with an item";
        public string drawerTip = "Move your mouse forward and backward to pull and push drawers. Left click to stop interacting with the drawer";
        public string cabinetTip = "Move your mouse left and right to swing open and close doors. For dishwashers and ovens, move your mouse forward and backward. Left click to stop interacting with the door";

        /// <summary>
        /// When scripts are recompiled resets the tips
        /// </summary>
        public void OnDisable()
        {
            gotMovementTip = false;
            gotHoldTip = false;
            gotInteractTip = false;
            gotDrawerTip = false;
            gotCabinetTip = false;
        }
    }
}
