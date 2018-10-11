using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Cyan
{
    /// <summary>
    /// Handles the UI elements for the tipsscreen
    /// </summary>
    public class TipsScreen : MonoBehaviour
    {
        public Interact player;
        public Hover playerHover;
        public float tipTime = 5f;

        public Image textBackground;
        public Image mouseImg;
        public Image wasdImg;
        public Image rkeyImg;
        public Text text;

        private KeyCode killTipKey = KeyCode.Return;
        public Tips tips;
        private IList<Image> uiElements = new List<Image>();

        private bool coroutineIsRunning;

        void Start()
        {
            player = FindObjectOfType<Interact>();
            playerHover = FindObjectOfType<Hover>();
            uiElements.Add(textBackground);
            uiElements.Add(mouseImg);
            uiElements.Add(wasdImg);
            uiElements.Add(rkeyImg);
            DeactivateImgs();

            if (tips.gotMovementTip == false)
            {
                giveTip(tips.movementTip);
                ActivateElements(true, true, false);
                tips.gotMovementTip = true;
                StartCoroutine("tipTimer");
            }
        }

        /// <summary>
        /// Timer for how long the tip stays on the screen. Once the coroutine finishes deactivates the UI elements
        /// </summary>
        /// <returns></returns>
        private IEnumerator tipTimer()
        {
            coroutineIsRunning = true;
            yield return new WaitForSeconds(tipTime);
            foreach (Image i in uiElements)
            {
                i.enabled = false;
            }
            text.enabled = false;
            coroutineIsRunning = false;
        }

        /// <summary>
        /// Sets the tip text on the UI text element
        /// </summary>
        /// <param name="tipname">The tip the player will recieve</param>
        public void giveTip(string tipname)
        {
            textBackground.enabled = true;
            text.text = tipname;
        }

        /// <summary>
        /// Activates the primary elements.,Tip background and tip text,  to the tips screen. 
        /// </summary>
        private void ActivateElements()
        {
            textBackground.enabled = true;
            text.enabled = true;
        }

        /// <summary>
        /// Deactivates all Images from the tipssreen
        /// </summary>
        private void DeactivateImgs()
        {
            textBackground.enabled = false;
            rkeyImg.enabled = false;
            wasdImg.enabled = false;
            mouseImg.enabled = false;
        }

        /// <summary>
        /// Activates visual aid images 
        /// </summary>
        /// <param name="showMouseImage">If the tip needs the mouse image, activates</param>
        /// <param name="showWASDImage">If the tip needs the WASD image, activates</param>
        /// <param name="showRkeyImage">If the tip needs the R key image, activates</param>
        private void ActivateElements(bool showMouseImage, bool showWASDImage, bool showRkeyImage)
        {
            ActivateElements();
            mouseImg.enabled = showMouseImage;
            wasdImg.enabled = showWASDImage;
            rkeyImg.enabled = showRkeyImage;
        }

        /// <summary>
        /// Checks if a coroutine is running and stops it
        /// </summary>
        private void KillTipTimerCoroutine()
        {
            if (coroutineIsRunning)
            {
                StopCoroutine("tipTimer");
                coroutineIsRunning = false;
            }
        }

        /// <summary>
        /// Checks what tips the player has recieved, makes sure that the tips do not repeat themselves.
        /// Once all tips have been recieved the script is deactivated along with all the other tip UI elements 
        /// </summary>
        private void CheckForTips()
        {
            // Hold Tip
            if (player.currentAction == InteractStates.isHolding && !tips.gotHoldTip)
            {
                KillTipTimerCoroutine();
                giveTip(tips.holdTip);
                ActivateElements(true, false, true);
                tips.gotHoldTip = true;
                StartCoroutine("tipTimer");

            }

            //Pulling Tips
            else if (player.currentAction == InteractStates.isPulling && player.selectedItem.tag.Contains("openable"))
            {
                try
                {
                    Open drawer = player.selectedItem.GetComponent<Open>();
                    // Activate Drawer Tip
                    if (drawer.hType.ToString().Contains("slide") && !tips.gotDrawerTip)
                    {
                        KillTipTimerCoroutine();
                        giveTip(tips.drawerTip);
                        ActivateElements(true, false, false);
                        tips.gotDrawerTip = true;
                        StartCoroutine("tipTimer");
                    }
                    // Activate Cabinet Tip
                    else if (!drawer.hType.ToString().Contains("slide") && !tips.gotCabinetTip)
                    {
                        KillTipTimerCoroutine();
                        giveTip(tips.cabinetTip);
                        ActivateElements(true, false, false);
                        tips.gotCabinetTip = true;
                        StartCoroutine("tipTimer");
                    }
                }
                catch
                {
                    Debug.Log("Can't find open script in openable object");
                }
            }

            // Hover interact tip
            else if (playerHover.canInteract && !tips.gotInteractTip)
            {
                KillTipTimerCoroutine();
                giveTip(tips.interactTip);
                ActivateElements(true, false, false);
                tips.gotInteractTip = true;
                StartCoroutine("tipTimer");
            }

            //Once all tips have been recieved, deactivate 
            else if (tips.gotCabinetTip && tips.gotDrawerTip && tips.gotDrawerTip && tips.gotHoldTip && tips.gotInteractTip && tips.gotMovementTip)
            {
                //Debug.Log("Got all the tips");
                StartCoroutine("tipTimer");
                text.enabled = false;
                textBackground.enabled = false;
                mouseImg.enabled = false;
                rkeyImg.enabled = false;
                wasdImg.enabled = false;
                this.enabled = false;
            }
        }

        /// <summary>
        /// Meant for early tip clearing, kills the tiptimer coroutine, clears text and deactivates an UI stuff
        /// </summary>
        private void ClearTips()
        {
            KillTipTimerCoroutine();
            text.text = "";
            DeactivateImgs();
        }


        // Update is called once per frame
        void Update()
        {

            // CheckForTips();
            if (!player.name.Contains("Elmer"))
            {
                if (Input.GetKeyDown(killTipKey))
                {
                    ClearTips();

                }
            }
        }
    }

}
